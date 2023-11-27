using Grpc.Core;
using Grpc.Core.Interceptors;
using WarehouseService.Constants;

namespace WarehouseService.Interceptors;

public class LoggerInterceptor : Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger;

    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>
    (
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        var requestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);

        _logger.LogInformation
        (
            LoggerMessagesConstants.MessageWithRequestData,
            MethodType.Unary,
            context.Method,
            requestData
        );

        var response = await continuation(request, context);

        var responseData = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        _logger.LogInformation
        (
            LoggerMessagesConstants.MessageWithResponseData,
            responseData
        );

        return response;
    }
}