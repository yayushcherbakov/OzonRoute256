using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Configuration;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Constants;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Helpers;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.ServiceBus.OrderEvents;

public class OrderEventBackgroundService : BackgroundService
{
    private readonly OrderEventConsumer<Ignore, OrderEvent> _consumer;
    private readonly ILogger<OrderEventBackgroundService> _logger;

    public OrderEventBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OrderEventBackgroundService> logger,
        IOptions<OrderEventConsumerOptions> options)
    {
        _logger = logger;

        var handler = serviceProvider.GetRequiredService<OrderEventHandler>();

        var kafkaConsumerOptions = options.Value;

        _consumer = new OrderEventConsumer<Ignore, OrderEvent>(
            handler,
            kafkaConsumerOptions.BootstrapServers,
            kafkaConsumerOptions.GroupId,
            kafkaConsumerOptions.OrderEvents,
            null,
            new SystemTextJsonDeserializer<OrderEvent>(),
            serviceProvider.GetRequiredService<ILogger<OrderEventConsumer<Ignore, OrderEvent>>>(),
            options);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.UnhandledErrorMessage);
        }
    }
}
