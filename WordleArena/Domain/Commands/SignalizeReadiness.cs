using Mediator;

namespace WordleArena.Domain.Commands;

public class SignalizeReadiness(GameId gameId, UserId userId, GameType gameType) : IRequest<SignalizeReadinessResponse>
{
    public GameId GameId { get; set; } = gameId;
    public UserId UserId { get; set; } = userId;

    public GameType GameType { get; set; } = gameType;
}