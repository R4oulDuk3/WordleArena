using Mediator;

namespace WordleArena.Domain.Queries;

public class GetPlayersInfo(List<UserId> userIds) : IRequest<List<PlayerInfo>>
{
    public List<UserId> UserIds { get; set; } = userIds;
}