﻿using System;
using System.Text.Json;
using Confluent.Kafka;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Constants;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Helpers;

public class SystemTextJsonDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SystemTextJsonDeserializer(JsonSerializerOptions? jsonSerializerOptions = null) =>
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new ArgumentNullException(nameof(data), ErrorMessages.CannotDeserializeNull);
        }

        return JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions)
               ?? throw new ArgumentException(
                   string.Format(
                       ErrorMessages.CannotDeserializeData,
                       typeof(T)));
    }
}
