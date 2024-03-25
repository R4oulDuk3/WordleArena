using LinqToDB;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class ActivateBotsHandler(IGrainFactory factory, ArenaDbContext db) : IRequestHandler<ActivateBots, List<UserId>>
{
    public async Task<List<UserId>> Handle(ActivateBots request, CancellationToken cancellationToken)
    {
        List<Bot> availableBots;
        await using (var transaction = await db.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                availableBots = await db.Bots.Where(b => !b.InUse).Take(request.Count)
                    .ToListAsync(token: cancellationToken);
                var selectedBotIds = availableBots.Select(b => b.UserId);
                await db.Bots.Where(b => selectedBotIds.Contains(b.UserId))
                    .ExecuteUpdateAsync(b => b.SetProperty(bot => bot.InUse, true), cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        if (availableBots.Count < request.Count)
        {
            var newBots = new List<Bot>();
            for (var i = 0; i < request.Count - availableBots.Count; i++)
            {
                var userId = UserId.GenerateBotUserId();
                var bot = new Bot(userId, userId.Id, true);
                await db.Bots.AddAsync(bot, cancellationToken);
                newBots.Add(bot);
            }

            await db.SaveChangesAsync(cancellationToken);
            availableBots.AddRange(newBots);
        }

        return availableBots.Select(b => b.UserId).ToList();
    }
}