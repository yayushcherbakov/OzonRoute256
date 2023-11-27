namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Configuration;

public class OrderEventConsumerOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public string OrderEvents { get; set; } = string.Empty;

    public int ChannelCapacity { get; set; }

    public int BufferDelayInSeconds { get; set; }

    public int MaxRetryAttempts { get; set; }

    public int RetryDelayInMilliseconds { get; set; }
}
