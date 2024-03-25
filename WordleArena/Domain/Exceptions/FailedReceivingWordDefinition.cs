namespace WordleArena.Domain.Exceptions;

public class FailedReceivingWordDefinition(string message) : Exception(message)
{
}