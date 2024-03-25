using MediatR;

namespace WordleArena.Domain.Events.UserSession;

public class UserEnteredMatchmaking : UserSessionEvent, INotification
{
    protected UserEnteredMatchmaking(UserId userId, GameType type) : base(userId)
    {
        GameType = type;
    }

    public GameType GameType { get; set; }
}