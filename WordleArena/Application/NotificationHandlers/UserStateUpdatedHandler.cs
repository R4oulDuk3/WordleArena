using Mediator;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserStateUpdatedHandler(IGrainFactory clusterClient) : INotificationHandler<UserStateUpdated>
{
    public async ValueTask Handle(UserStateUpdated notification, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(notification.UserId.Id);
        await userSessionGrain.Handle(notification);
    }
}