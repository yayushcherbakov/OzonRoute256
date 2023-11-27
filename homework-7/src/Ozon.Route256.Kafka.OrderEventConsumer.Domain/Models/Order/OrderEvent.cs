using System;
using System.Text.Json.Serialization;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Enums;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;

public class OrderEvent
{
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("warehouse_id")]
    public long WarehouseId { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; }

    [JsonPropertyName("moment")]
    public DateTime Moment { get; set; }

    [JsonPropertyName("positions")]
    public OrderEventPosition[] Positions { get; set; } = null!;
}
