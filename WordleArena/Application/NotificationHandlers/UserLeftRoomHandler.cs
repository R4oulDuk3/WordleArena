using MediatR;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserLeftRoomHandler
    (ILogger<UserLeftRoomHandler> logger, IGrainFactory clusterClient) : INotificationHandler<UserLeftRoom>
{
    public async Task Handle(UserLeftRoom notification, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(notification.UserId.Id);
        await userSessionGrain.Handle(notification);
    }
}