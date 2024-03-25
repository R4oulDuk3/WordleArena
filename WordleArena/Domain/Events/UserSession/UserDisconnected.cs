using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserDisconnected : UserSessionEvent, INotification
{
    public UserDisconnected(UserId player) : base(player)
    {
    }
}