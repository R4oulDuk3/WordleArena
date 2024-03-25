using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class GameFinished(UserId userId, GameType gameType, GameId gameId) : UserSessionEvent(userId), INotification
{
    [Id(0)] public GameId GameId { get; set; } = gameId;
    [Id(1)] public GameType GameType { get; set; } = gameType;
}