using Mediator;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetMatchHistoryHandler(ArenaDbContext dbContext) : IRequestHandler<GetMatchHistory, MatchHistory>
{
    public async ValueTask<MatchHistory> Handle(GetMatchHistory request, CancellationToken cancellationToken)
    {
        var results = await dbContext.TempoGamePlayerResults.Where(r => r.UserId.Equals(request.UserId))
            .OrderByDescending(r => r.FinishedAt).Skip(request.Offset)
            .Take(request.Limit).ToListAsync(cancellationToken);
        var matchHistoryRecords = results
            .Select(r =>
                new MatchHistoryRecord(r.UserId, r.GameId, r.FinishedAt, r.ResultInfo.Place, r.ResultInfo.Score))
            .ToList();
        var totalCount =
            await dbContext.TempoGamePlayerResults.CountAsync(r => r.UserId.Equals(request.UserId), cancellationToken);
        return new MatchHistory(matchHistoryRecords, request.Offset / request.Limit, totalCount);
    }
}