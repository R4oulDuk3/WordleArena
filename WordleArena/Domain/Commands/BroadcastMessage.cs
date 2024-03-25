using System.Reflection;
using Mediator;

namespace WordleArena.Domain.Commands;

public class BroadcastMessage(string method, object message) : IRequest
{
    public string Method = method;
    public object Message = message;
}