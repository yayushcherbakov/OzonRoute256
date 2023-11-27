namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Configurations;

public record DalOptions
{
    public string PostgresConnectionString { get; init; } = string.Empty;
}
