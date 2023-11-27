namespace WarehouseService.Constants;

internal static class LoggerMessagesConstants
{
    public const string MessageWithRequestData =
        "Starting receiving call. Type/Method: {Type} / {Method}. Request data: {RequestData}";

    public const string MessageWithResponseData =
        "End of call reception. Response data: {ResponseData}";

    public const string ExceptionMessage =
        "An exception has been thrown: {ExceptionMessage}";
}