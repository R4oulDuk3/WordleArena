using Mediator;

namespace WordleArena.Domain.Queries;

public class GetMatchHistory(UserId userId, int limit, int offset): IRequest<MatchHistory>
{
    public UserId UserId = userId;
    public int Limit = limit;
    public int Offset = offset;
}