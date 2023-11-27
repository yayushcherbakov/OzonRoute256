using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<IOrderService, OrderService>();

        return services;
    }
}
