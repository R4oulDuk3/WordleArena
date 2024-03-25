using Mediator;

namespace WordleArena.Domain.Queries;

public class GetPlayerState(GameId gameId, UserId userId, GameType gameType) : IRequest<GamePlayerState>
{
    public readonly GameId GameId = gameId;
    public readonly GameType GameType = gameType;
    public readonly UserId UserId = userId;
}