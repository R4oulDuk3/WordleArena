using Mediator;

namespace WordleArena.Domain.Queries;

public class GetGameStatus(GameId gameId, GameType gameType) : IRequest<GameStatus>
{
    public GameId GameId { get; set; } = gameId;
    public GameType GameType { get; set; } = gameType;
}