using MediatR;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetTempoGamePlayerResultQuery
    (IGrainFactory factory, ArenaDbContext dbContext) : IRequestHandler<GetTempoGameResult, List<TempoGamePlayerResult>>
{
    public async Task<List<TempoGamePlayerResult>> Handle(GetTempoGameResult request,
        CancellationToken cancellationToken)
    {
        var playerResults = await dbContext.TempoGamePlayerResults.Where(result => result.GameId.Equals(request.GameId))
            .ToListAsync(cancellationToken);
        return playerResults;
    }
}