using Mediator;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class GameFinishedHandler(IGrainFactory clusterClient) : INotificationHandler<GameFinished>
{
    public async ValueTask Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        if (notification.UserId.IsBot()) return;
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(notification.UserId.Id);
        await userSessionGrain.Handle(notification);
    }
}