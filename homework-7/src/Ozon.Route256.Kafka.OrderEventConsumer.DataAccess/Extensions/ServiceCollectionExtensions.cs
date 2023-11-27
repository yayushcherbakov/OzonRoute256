using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Configurations;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(
        this IServiceCollection services)
    {
        AddPostgresRepositories(services);

        return services;
    }

    private static void AddPostgresRepositories(IServiceCollection services)
    {
        services.AddTransient<IItemRepository, ItemRepository>();
    }

    public static IServiceCollection AddDalInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        //read config
        services.Configure<DalOptions>(config.GetSection(nameof(DalOptions)));

        //configure postrges types
        Infrastructure.Postgres.MapCompositeTypes();

        //add migrations
        Infrastructure.Postgres.AddMigrations(services);

        return services;
    }
}
