using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Contracts;

public interface IOrderService
{
    public Task UpdateProductAccounting(OrderEvent[] events, CancellationToken cancellationToken);
}
