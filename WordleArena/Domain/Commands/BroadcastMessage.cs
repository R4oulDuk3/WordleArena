using MediatR;

namespace WordleArena.Domain.Commands;

public class BroadcastMessage(string method, object message) : IRequest
{
    public object Message = message;
    public string Method = method;
}