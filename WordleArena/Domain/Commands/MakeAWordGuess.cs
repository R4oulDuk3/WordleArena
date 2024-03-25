using MediatR;
using WordleArena.Domain.Events.Game;

namespace WordleArena.Domain.Commands;

public class MakeAWordGuess(UserId userId, GameType gameType, GameId gameId, Guess guess)
    : IRequest<(GameEvent eventOccured, GamePlayerState currentPlayerState)>
{
    public GameId GameId { get; } = gameId;
    public GameType GameType { get; } = gameType;
    public Guess Guess { get; } = guess;
    public UserId UserId { get; } = userId;
}