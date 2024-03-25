using MediatR;

namespace WordleArena.Domain.Queries;

public class GetMatchHistory(UserId userId, int limit, int offset) : IRequest<MatchHistory>
{
    public int Limit = limit;
    public int Offset = offset;
    public UserId UserId = userId;
}