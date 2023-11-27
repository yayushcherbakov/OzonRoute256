namespace Domain.Constants;

internal static class ErrorMessages
{
    public const string SalesNotFound = "Sales not found by id: {0}";

    public const string SeasonsNotFound = "Seasons coefficient not found by id ({0}) and month ({1})";

    public const string DateMustBeGreaterThanOrEqualToCurrentDate
        = "The date must be greater than or equal to the current date";
}