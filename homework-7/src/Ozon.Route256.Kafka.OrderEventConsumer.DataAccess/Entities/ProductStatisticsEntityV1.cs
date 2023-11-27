namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Entities;

public record  ProductStatisticsEntityV1
{
    public long Id { get; init; }

    public long ItemId { get; init; }

    public long CreatedCount { get; set; }

    public long DeliveredCount { get; set; }

    public long CancelledCount { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }
}
