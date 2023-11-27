using Dapper;
using Microsoft.Extensions.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Configurations;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Repositories;

public sealed class ItemRepository : PgRepository, IItemRepository
{
    public ItemRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(ProductStatisticsEntityV1[] statistics, CancellationToken token)
    {
        const string sqlQuery = @"
   insert into product_statistics (item_id
        , created_count
        , delivered_count
        , cancelled_count
        , modified_at)
   select item_id
        , created_count
        , delivered_count
        , cancelled_count
        , modified_at
     from UNNEST(@Statistics)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Statistics = statistics
                },
                cancellationToken: token));

        return ids.ToArray();
    }

    public async Task<ProductStatisticsEntityV1[]> Get(long[] itemIds, CancellationToken token)
    {
        var baseSql = @"
select id
     , item_id
     , created_count
     , delivered_count
     , cancelled_count
     , modified_at
from product_statistics
";

        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (itemIds.Any())
        {
            conditions.Add($"item_id = ANY(@ItemIds)");
            @params.Add($"ItemIds", itemIds);
        }

        var cmd = new CommandDefinition(
            baseSql + $" where {string.Join(" and ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();
        return (await connection.QueryAsync<ProductStatisticsEntityV1>(cmd))
            .ToArray();
    }

    public async Task Update(ProductStatisticsEntityV1 statistic, CancellationToken token)
    {
        const string sqlQuery = @"
update product_statistics
   set created_count = @CreatedCount
, delivered_count = @DeliveredCount
, cancelled_count = @CancelledCount
, modified_at = @ModifiedAt
where item_id = @ItemId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ItemId = statistic.ItemId,
                    CreatedCount = statistic.CreatedCount,
                    DeliveredCount = statistic.DeliveredCount,
                    CancelledCount = statistic.CancelledCount,
                    ModifiedAt = DateTimeOffset.UtcNow,
                },
                cancellationToken: token));
    }
}
