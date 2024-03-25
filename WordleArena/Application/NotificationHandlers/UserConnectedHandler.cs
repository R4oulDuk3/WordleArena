using Mediator;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserConnectedHandler
    (ILogger<UserConnectedHandler> logger, IGrainFactory clusterClient) : INotificationHandler<UserConnected>
{
    public async ValueTask Handle(UserConnected userConnectedEvent, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(userConnectedEvent.UserId.Id);
        await userSessionGrain.Handle(userConnectedEvent);
        await userSessionGrain.Start();
        var connectedUsersCountGrain = clusterClient.GetGrain<IConnectedUsersCountGrain>(Guid.Empty);
        await connectedUsersCountGrain.Handle(userConnectedEvent);
    }
}