using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserConnected : UserSessionEvent, INotification
{
    public UserConnected(UserId userId, string connectionId, string hostIpAddress) : base(userId)
    {
        ConnectionId = connectionId;
        HostIpAddress = hostIpAddress;
    }

    [Id(2)] public string ConnectionId { get; init; }

    [Id(3)] public string HostIpAddress { get; init; }
}