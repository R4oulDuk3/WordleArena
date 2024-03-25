using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserStateUpdated : UserSessionEvent, INotification
{
    public UserStateUpdated(UserId userId, UserState userState) : base(userId)
    {
        UserState = userState;
    }

    [Id(1)] public UserState UserState { get; set; }
}