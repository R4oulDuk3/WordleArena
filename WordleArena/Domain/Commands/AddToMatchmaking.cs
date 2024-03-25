using Mediator;

namespace WordleArena.Domain.Commands;

public class AddToMatchmaking(UserId userId, GameType gameType) : IRequest
{
    public GameType GameType { get; set; } = gameType;
    public UserId UserId { get; set; } = userId;
}