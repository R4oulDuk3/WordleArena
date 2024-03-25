using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain.Events.UserSession;

namespace WordleArena.Domain;

[ExportTsEnum(OutputDir = "domain")]
public enum UserState
{
    InMatchmaking,
    InMenu,
    InGame
}

[GenerateSerializer]
public class ParticipatingGame(GameId gameId, GameType gameType)
{
    [Id(0)] public GameId GameId { get; set; } = gameId;
    [Id(1)] public GameType GameType { get; set; } = gameType;
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class UserSession
{
    [Id(5)] public List<ParticipatingGame> ActiveParticipatingGamesIds { get; set; } = new();
    [Id(0)] public int Version { get; set; }

    [Id(1)] public UserId UserId { get; set; }

    [Id(2)] public string? ConnectionId { get; set; }

    [Id(3)] public string? HostIpAddress { get; set; }

    [Id(4)] public bool IsConnected { get; set; }

    [Id(6)] public UserState UserState { get; set; }

    [Id(7)] public RoomId? RoomId { get; set; }


    public void Apply(UserConnected @event)
    {
        Version++;
        UserId = @event.UserId;
        ConnectionId = @event.ConnectionId;
        HostIpAddress = @event.HostIpAddress;
        IsConnected = true;
    }

    public void Apply(UserDisconnected @event)
    {
        Version++;
        UserId = @event.UserId;
        ConnectionId = null;
        HostIpAddress = null;
        IsConnected = false;
    }

    public void Apply(UserJoinedGame @event)
    {
        Version++;
        ActiveParticipatingGamesIds.RemoveAll(pg => Equals(pg.GameId, @event.GameId));
        ActiveParticipatingGamesIds.Add(new ParticipatingGame(@event.GameId, @event.GameType));
    }

    public void Apply(UserJoinedRoom @event)
    {
        Version++;
        RoomId = @event.RoomId;
    }

    public void Apply(UserLeftRoom @event)
    {
        Version++;
        RoomId = null;
    }


    public void Apply(UserLeftGame @event)
    {
        Version++;
        ActiveParticipatingGamesIds.RemoveAll(pg => Equals(pg.GameId, @event.GameId));
    }

    public void Apply(GameFinished @event)
    {
        Version++;
        ActiveParticipatingGamesIds.RemoveAll(pg => Equals(pg.GameId, @event.GameId));
    }

    public void Apply(UserEnteredMatchmaking @event)
    {
        Version++;
        UserState = UserState.InMatchmaking;
    }

    public void Apply(UserStateUpdated @event)
    {
        Version++;
        UserState = @event.UserState;
    }
}