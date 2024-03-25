namespace WordleArena.Domain.Exceptions;

public class RetryableException(string message, Exception rootException) : Exception(message, rootException)
{
    public Exception RootException { get; } = rootException;
}