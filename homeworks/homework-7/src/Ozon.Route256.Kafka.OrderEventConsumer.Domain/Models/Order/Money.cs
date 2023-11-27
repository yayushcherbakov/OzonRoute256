using System.Text.Json.Serialization;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;

public sealed class Money
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("units")]
    public long Units { get; set; }

    [JsonPropertyName("nanos")]
    public int Nanos { get; set; }
}
