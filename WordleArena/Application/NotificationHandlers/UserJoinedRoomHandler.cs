using Mediator;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserJoinedRoomHandler
    (ILogger<UserJoinedGameHandler> logger, IGrainFactory clusterClient) : INotificationHandler<UserJoinedRoom>
{
    public async ValueTask Handle(UserJoinedRoom notification, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(notification.UserId.Id);
        await userSessionGrain.Handle(notification);
    }
}