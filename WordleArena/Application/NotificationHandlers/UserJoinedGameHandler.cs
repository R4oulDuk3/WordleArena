using Mediator;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserJoinedGameHandler
    (ILogger<UserJoinedGameHandler> logger, IGrainFactory clusterClient) : INotificationHandler<UserJoinedGame>
{
    public async ValueTask Handle(UserJoinedGame notification, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(notification.UserId.Id);
        await userSessionGrain.Handle(notification);
    }
}