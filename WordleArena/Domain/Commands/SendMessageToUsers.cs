using MediatR;

namespace WordleArena.Domain.Commands;

public class SendMessageToUsers(object message, string method, List<UserId> userIds) : IRequest
{
    public object Message = message;
    public string Method = method;
    public List<UserId> UserIds = userIds;
}