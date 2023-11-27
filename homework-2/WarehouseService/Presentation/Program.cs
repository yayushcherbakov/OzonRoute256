using DataAccess.Extensions;
using Domain.Extensions;
using Domain.Models;
using WarehouseService.GrpcServices;
using WarehouseService.Interceptors;
using WarehouseService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program), typeof(Product));

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggerInterceptor>();
    options.Interceptors.Add<ErrorHandlerInterceptor>();
});

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