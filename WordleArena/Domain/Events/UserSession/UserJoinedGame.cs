using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserJoinedGame : UserSessionEvent, INotification
{
    public UserJoinedGame(UserId player, GameId gameId, GameType gameType) : base(player)
    {
        GameId = gameId;
        GameType = gameType;
    }

    [Id(1)] public GameId GameId { get; init; }
    [Id(0)] public GameType GameType { get; init; }
}