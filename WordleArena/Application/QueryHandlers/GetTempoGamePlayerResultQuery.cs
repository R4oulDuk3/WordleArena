using Mediator;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetTempoGamePlayerResultQuery
    (IGrainFactory factory, ArenaDbContext dbContext) : IRequestHandler<GetTempoGameResult, List<TempoGamePlayerResult>>
{
    public async ValueTask<List<TempoGamePlayerResult>> Handle(GetTempoGameResult request,
        CancellationToken cancellationToken)
    {
        var playerResults = await dbContext.TempoGamePlayerResults.Where(result => result.GameId.Equals(request.GameId))
            .ToListAsync(cancellationToken);
        return playerResults;
    }
}