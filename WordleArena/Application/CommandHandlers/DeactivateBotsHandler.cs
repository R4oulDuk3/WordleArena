using Mediator;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class DeactivateBotsHandler(ArenaDbContext context) : IRequestHandler<DeactivateBots>
{
    public async ValueTask<Unit> Handle(DeactivateBots request, CancellationToken cancellationToken)
    {
        await context.Bots.Where(b => request.BotIds.Contains(b.UserId))
            .ExecuteUpdateAsync(b => b.SetProperty(bot => bot.InUse, false), cancellationToken);

        return Unit.Value;
    }
}