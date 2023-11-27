using System.Text.Json.Serialization;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;

public sealed class OrderEventPosition
{
    [JsonPropertyName("item_id")]
    public long ItemId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    public Money Price { get; set; } = null!;
}
