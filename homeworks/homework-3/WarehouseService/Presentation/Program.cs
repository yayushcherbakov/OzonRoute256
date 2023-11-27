using System.Reflection;
using Calzolari.Grpc.AspNetCore.Validation;
using DataAccess.Extensions;
using Domain.Extensions;
using Domain.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using WarehouseService.GrpcServices;
using WarehouseService.Interceptors;
using WarehouseService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager.Enabled = false;

builder.Services.AddControllers()
    .AddFluentValidation(options =>
    {
        options.ImplicitlyValidateChildProperties = true;
        options.ImplicitlyValidateRootCollectionElements = true;
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

builder.Services.AddAutoMapper(typeof(Program), typeof(Product));

builder.Services.AddGrpc(options =>
{
    options.EnableMessageValidation();
    options.Interceptors.Add<LoggerInterceptor>();
    options.Interceptors.Add<ErrorHandlerInterceptor>();
});

builder.Services.AddGrpcValidation();
builder.Services.AddGrpcReflection();
builder.Services.AddSwaggerGen();

builder.Services
    .AddEndpointsApiExplorer()
    .AddDomainServices()
    .AddDataAccess();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapGrpcService<GrpcWarehouseService>();

app.Run();

public partial class Program
{
}