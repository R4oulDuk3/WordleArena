using Mediator;

namespace WordleArena.Domain.Commands;

public class ApplyEffect(UserId sender, Effect effect, GameType gameType, GameId gameId) : IRequest<GamePlayerState>
{
    public Effect Effect { get; set; } = effect;
    public GameId GameId { get; set; } = gameId;
    public GameType GameType { get; set; } = gameType;
    public UserId Sender { get; set; } = sender;
}