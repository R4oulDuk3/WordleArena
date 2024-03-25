using MediatR;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class RemoveFromMatchmakingHandler(ArenaDbContext dbContext) : IRequestHandler<RemoveFromMatchmaking>
{
    public async Task Handle(RemoveFromMatchmaking request, CancellationToken cancellationToken)
    {
        await dbContext.PlayerMatchmakingInfos.Where(info => request.UserIds.Contains(info.UserId))
            .ExecuteDeleteAsync(cancellationToken);
    }
}