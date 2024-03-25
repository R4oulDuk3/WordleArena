namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserSessionEvent
{
    protected UserSessionEvent(UserId userId)
    {
        UserId = userId;
    }

    [Id(0)] public UserId UserId { get; init; }
}