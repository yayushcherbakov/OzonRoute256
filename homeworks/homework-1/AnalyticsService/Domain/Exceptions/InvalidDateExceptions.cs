namespace Domain.Exceptions;

public class InvalidDateExceptions : Exception
{
    public InvalidDateExceptions(string message) : base(message)
    {
    }
}