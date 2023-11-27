using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Extensions;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Extensions;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddDalInfrastructure(_configuration);
        services.AddDalRepositories();
        services.AddDomainServices();
        services.AddOrderEventHandler(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
