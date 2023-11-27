using Newtonsoft.Json;
using System.Net;
using AnalyticsService.Constants;

namespace AnalyticsService.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
        }
    }
    
    private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)  
    {
        const int statusCode = (int)HttpStatusCode.InternalServerError;
        
        var result = JsonConvert.SerializeObject(new  
        {  
            StatusCode = statusCode,  
            ErrorMessage = exception.Message  
        });  
        
        context.Response.ContentType = ContentTypeConstants.ApplicationJson;  
        context.Response.StatusCode = statusCode; 
        
        return context.Response.WriteAsync(result);  
    }
}