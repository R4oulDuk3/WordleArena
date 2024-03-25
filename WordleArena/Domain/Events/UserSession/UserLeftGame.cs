using MediatR;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserLeftGame : UserSessionEvent, INotification
{
    public UserLeftGame(UserId player, GameId gameId, GameType gameType) : base(player)
    {
        GameId = gameId;
        GameType = gameType;
    }

    [Id(1)] public GameId GameId { get; init; }
    [Id(0)] public GameType GameType { get; init; }
}