﻿using Domain.Contracts;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services.AddScoped<IWarehouseService, WarehouseService>();
    }
}