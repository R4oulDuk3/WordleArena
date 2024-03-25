using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class AddToMatchmakingHandler(ArenaDbContext db) : IRequestHandler<AddToMatchmaking>
{
    public async Task Handle(AddToMatchmaking request, CancellationToken cancellationToken)
    {
        await db.PlayerMatchmakingInfos.AddAsync(
            new PlayerMatchmakingInfo(request.UserId, request.GameType, DateTime.UtcNow), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}