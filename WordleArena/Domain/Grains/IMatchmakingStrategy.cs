using Mediator;

namespace WordleArena.Domain.Grains;

public interface IMatchmakingStrategy
{
    public Task<List<List<UserId>>>
        MatchmakePlayers(IMediator mediator, GameType gameType);
}