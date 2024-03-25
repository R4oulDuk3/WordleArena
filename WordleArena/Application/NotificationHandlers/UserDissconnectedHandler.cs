using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Application.NotificationHandlers;

public class UserDisconnectedHandler
(ILogger<UserConnectedHandler> logger, IMediator mediator, IGrainFactory factory,
    IGrainFactory clusterClient) : INotificationHandler<UserDisconnected>
{
    public async Task Handle(UserDisconnected userDisconnectedEvent, CancellationToken cancellationToken)
    {
        var connectedUsersCountGrain = clusterClient.GetGrain<IConnectedUsersCountGrain>(Guid.Empty);
        await connectedUsersCountGrain.Handle(userDisconnectedEvent);
        await mediator.Send(new RemoveFromMatchmaking(new List<UserId> { userDisconnectedEvent.UserId }),
            cancellationToken);
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(userDisconnectedEvent.UserId.Id);
        var session = await userSessionGrain.GetUserSession();
        if (session.RoomId != null)
        {
            var roomGrain = factory.GetGrain<IRoomGrain>(session.RoomId.Id);
            var status = await roomGrain.GetRoomStatus();
            if (status == RoomStatus.NotInitialized) await roomGrain.Leave(userDisconnectedEvent.UserId);
            await mediator.Publish(new UserLeftRoom(userDisconnectedEvent.UserId, session.RoomId), cancellationToken);
        }

        await userSessionGrain.Handle(userDisconnectedEvent);
        await userSessionGrain.Stop();
    }
}