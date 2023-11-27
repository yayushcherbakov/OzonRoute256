using Grpc.Core;
using Grpc.Core.Interceptors;
using WarehouseService.Constants;

namespace WarehouseService.Interceptors;

public class ErrorHandlerInterceptor : Interceptor
{
    private readonly ILogger<ErrorHandlerInterceptor> _logger;

    public ErrorHandlerInterceptor(ILogger<ErrorHandlerInterceptor> logger)
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
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            _logger.LogInformation
            (
                LoggerMessagesConstants.ExceptionMessage,
                ex.Message
            );

            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}