using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Contracts;

public interface IItemRepository
{
    public Task<long[]> Add(ProductStatisticsEntityV1[] statistics, CancellationToken token);

    public Task<ProductStatisticsEntityV1[]> Get(long[] itemIds, CancellationToken token);

    public Task Update(ProductStatisticsEntityV1 statistic, CancellationToken token);
}
