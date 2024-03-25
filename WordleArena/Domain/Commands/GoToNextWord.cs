using Mediator;

namespace WordleArena.Domain.Commands;

public class GoToNextWord(UserId userId, GameId gameId, GameType gameType) : IRequest<GamePlayerState>
{
    public UserId UserId { get; set; } = userId;
    public GameId GameId { get; set; } = gameId;
    public GameType GameType { get; set; } = gameType;
}