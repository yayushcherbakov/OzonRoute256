using DataAccess.Contracts;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        return services
            .AddScoped<ISalesRepository, SalesRepository>()
            .AddScoped<ISeasonsRepository, SeasonsRepository>();
    }
}