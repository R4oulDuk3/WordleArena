using Mediator;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class RemoveFromMatchmakingHandler(ArenaDbContext dbContext) : IRequestHandler<RemoveFromMatchmaking>
{
    public async ValueTask<Unit> Handle(RemoveFromMatchmaking request, CancellationToken cancellationToken)
    {
        await dbContext.PlayerMatchmakingInfos.Where(info => request.UserIds.Contains(info.UserId))
            .ExecuteDeleteAsync(cancellationToken);
        return Unit.Value;
    }
}