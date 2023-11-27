using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Constants;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.ServiceBus.OrderEvents;

public class OrderEventHandler : IHandler<Ignore, OrderEvent>
{
    private readonly ILogger<OrderEventHandler> _logger;

    private readonly IOrderService _orderService;

    public OrderEventHandler(ILogger<OrderEventHandler> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, OrderEvent>> messages, CancellationToken token)
    {
        var orders = messages
            .Select(x => x.Message.Value)
            .ToArray();

        await _orderService.UpdateProductAccounting(orders, token);

        _logger.LogInformation(LogMessages.HandledCountMessages, messages.Count);
    }
}
