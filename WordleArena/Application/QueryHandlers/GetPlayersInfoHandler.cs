using LinqToDB.EntityFrameworkCore;
using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetPlayersInfoHandler(ArenaDbContext context) : IRequestHandler<GetPlayersInfo, List<PlayerInfo>>
{
    public async ValueTask<List<PlayerInfo>> Handle(GetPlayersInfo request, CancellationToken cancellationToken)
    {
        var playersIds = request.UserIds.Where(id => id.IsHuman()).ToList();
        var botIds = request.UserIds.Where(id => id.IsBot()).ToList();

        var players = await context.Users.Where(u => playersIds.Contains(u.UserId)).ToListAsyncEF(cancellationToken);
        var bots = await context.Bots.Where(b => botIds.Contains(b.UserId)).ToListAsyncEF(cancellationToken);
        var playerInfo = players.Select(p => new PlayerInfo(p.UserId, p.Username)).ToList();
        playerInfo.AddRange(bots.Select(b => new PlayerInfo(b.UserId, b.Username)));
        return playerInfo;
    }
}