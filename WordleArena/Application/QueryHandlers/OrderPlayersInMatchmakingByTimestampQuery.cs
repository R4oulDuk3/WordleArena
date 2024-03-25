using LinqToDB;
using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class OrderPlayersInMatchmakingByTimestampQuery
    (ArenaDbContext dbContext) : IRequestHandler<OrderPlayersInMatchmakingByTimestamp, List<PlayerMatchmakingInfo>>
{
    public async Task<List<PlayerMatchmakingInfo>> Handle(OrderPlayersInMatchmakingByTimestamp request,
        CancellationToken cancellationToken)
    {
        return await dbContext.PlayerMatchmakingInfos.Where(info => info.Type == request.GameType)
            .OrderBy(info => info.EnterTimestamp).Take(request.Limit).ToListAsync(cancellationToken);
    }
}