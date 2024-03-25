using Mediator;

namespace WordleArena.Domain.Events.UserSession;

[GenerateSerializer]
public class UserLeftRoom : UserSessionEvent, INotification
{
    public UserLeftRoom(UserId player, RoomId roomId) : base(player)
    {
        RoomId = roomId;
    }

    [Id(0)] public RoomId RoomId { get; init; }
}