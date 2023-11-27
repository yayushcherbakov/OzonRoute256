using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Configuration;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.ServiceBus.OrderEvents;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddOrderEventHandler(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<OrderEventConsumerOptions>(config.GetSection(nameof(OrderEventConsumerOptions)));

        services.AddSingleton<OrderEventHandler>();

        services.AddHostedService<OrderEventBackgroundService>();

        return services;
    }

}
