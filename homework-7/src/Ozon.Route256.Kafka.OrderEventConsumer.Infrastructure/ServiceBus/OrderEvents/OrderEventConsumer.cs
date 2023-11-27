using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Configuration;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Constants;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Extensions;
using Polly;
using Polly.Retry;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.ServiceBus.OrderEvents;

public sealed class OrderEventConsumer<TKey, TValue> : IDisposable
{
    private readonly OrderEventConsumerOptions _orderEventConsumerOptions;

    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;

    private readonly ILogger<OrderEventConsumer<TKey, TValue>> _logger;

    public OrderEventConsumer(
        IHandler<TKey, TValue> handler,
        string bootstrapServers,
        string groupId,
        string topic,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer,
        ILogger<OrderEventConsumer<TKey, TValue>> logger,
        IOptions<OrderEventConsumerOptions> options)
    {
        _orderEventConsumerOptions = options.Value;

        var builder = new ConsumerBuilder<TKey, TValue>(
            new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            });

        if (keyDeserializer is not null)
        {
            builder.SetKeyDeserializer(keyDeserializer);
        }

        if (valueDeserializer is not null)
        {
            builder.SetValueDeserializer(valueDeserializer);
        }

        _handler = handler;
        _logger = logger;

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(_orderEventConsumerOptions.ChannelCapacity)
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        _consumer = builder.Build();
        _consumer.Subscribe(topic);
    }

    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }

    private async Task HandleCore(CancellationToken token)
    {
        await Task.Yield();

        await foreach (var consumeResults in _channel.Reader
                           .ReadAllAsync(token)
                           .Buffer(
                               _orderEventConsumerOptions.ChannelCapacity,
                               TimeSpan.FromSeconds(_orderEventConsumerOptions.BufferDelayInSeconds))
                           .WithCancellation(token))
        {
            token.ThrowIfCancellationRequested();

            var strategy = new ResiliencePipelineBuilder().AddRetry(
                new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    MaxRetryAttempts = _orderEventConsumerOptions.MaxRetryAttempts,
                    Delay = TimeSpan.FromMilliseconds(_orderEventConsumerOptions.RetryDelayInMilliseconds),
                    OnRetry = args =>
                    {
                        var exception = args.Outcome.Exception!;
                        _logger.LogError(LogMessages.RetryErrorMessage, exception.Message);
                        return default;
                    }
                }).Build();

            while (true)
            {
                try
                {
                    await strategy.ExecuteAsync(
                        async cancellationToken => { await _handler.Handle(consumeResults, cancellationToken); },
                        token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LogMessages.UnhandledErrorMessage);

                    continue;
                }

                var partitionLastOffsets = consumeResults
                    .GroupBy(
                        r => r.Partition.Value,
                        (_, f) => f.MaxBy(p => p.Offset.Value));

                foreach (var partitionLastOffset in partitionLastOffsets)
                {
                    strategy.Execute(_consumer.StoreOffset, partitionLastOffset);
                }

                break;
            }
        }
    }

    private async Task ConsumeCore(CancellationToken token)
    {
        await Task.Yield();

        while (_consumer.Consume(token) is { } result)
        {
            await _channel.Writer.WriteAsync(result, token);
            _logger.LogTrace(
                "{Partition}:{Offset}:WriteToChannel",
                result.Partition.Value,
                result.Offset.Value);
        }

        _channel.Writer.Complete();
    }

    public void Dispose() => _consumer.Close();
}
