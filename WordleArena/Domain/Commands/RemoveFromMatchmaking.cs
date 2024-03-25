using MediatR;

namespace WordleArena.Domain.Commands;

public class RemoveFromMatchmaking(List<UserId> userIds) : IRequest
{
    public List<UserId> UserIds { get; set; } = userIds;
}