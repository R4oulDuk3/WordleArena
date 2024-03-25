using Mediator;
using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public interface IRoomGrain : IGrainWithStringKey
{
    Task<bool> IsPlayerInRoom(UserId userId);
    Task Start(RoomId roomId, GameType gameType);
    Task End();
    Task<(bool success, string message)> Join(UserId userId);
    Task<(bool success, string message)> Leave(UserId userId);
    Task<(bool success, string message)> UpdatePlayerReadiness(UserId userId, bool isReady);
    Task BroadcastRoomState();
    Task<RoomStatus> GetRoomStatus();
    Task<RoomState> GetRoomState();
    Task Tick();
}

public class RoomGrain(IMediator mediator, IGrainFactory grainFactory) : Grain<RoomState>, IRoomGrain
{
    public static List<int> ViablePlayerCount = new() { 2, 4 };
    public static TimeSpan TimeUntilRouteToGame = TimeSpan.FromSeconds(5);
    public static TimeSpan StopAfter = TimeSpan.FromMinutes(10);
    private readonly TimeSpan tickPeriod = TimeSpan.FromMilliseconds(500);
    public RoomStatus RoomStatus;
    private IDisposable timer;

    public Task End()
    {
        RoomStatus = RoomStatus.NotInitialized;
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public async Task<(bool success, string message)> Leave(UserId userId)
    {
        State.ReadyPlayers.Remove(userId);
        State.PlayerInfos.RemoveAll(pi => pi.UserId.Equals(userId));
        await WriteStateAsync();
        await BroadcastRoomState();
        return (success: true, message: "Player successfully left");
    }

    public async Task<(bool success, string message)> UpdatePlayerReadiness(UserId userId, bool isReady)
    {
        if (State.PlayerInfos.Count(pi => pi.UserId.Equals(userId)) == 0)
            return (success: false, message: "You are not a participant of this room");

        if (isReady)
            State.ReadyPlayers.Add(userId);
        else
            State.ReadyPlayers.Remove(userId);

        await WriteStateAsync();
        await BroadcastRoomState();
        return (success: true, message: "Success!");
    }

    public async Task BroadcastRoomState()
    {
        var userIds = State.PlayerInfos.Select(pi => pi.UserId).ToList();
        await mediator.Send(new SendMessageToUsers(State, $"roomState:{State.RoomId.Id}", userIds));
    }

    public Task<RoomStatus> GetRoomStatus()
    {
        return Task.FromResult(RoomStatus);
    }

    public Task<RoomState> GetRoomState()
    {
        return Task.FromResult(State);
    }

    public Task<bool> IsPlayerInRoom(UserId userId)
    {
        return Task.FromResult(State.PlayerInfos.Count(pi => pi.UserId.Equals(userId)) == 1);
    }

    public async Task Start(RoomId roomId, GameType gameType)
    {
        RoomStatus = RoomStatus.InProgress;
        var gameId = GameId.NewGameId();
        State = new RoomState(TimeUntilRouteToGame, roomId, gameId, gameType);
        await WriteStateAsync();
        RegisterTimer(TickHandler, null, tickPeriod,
            tickPeriod);
    }

    public async Task Tick()
    {
        if (State.PlayerInfos.Count == State.ReadyPlayers.Count && ViablePlayerCount.Contains(State.PlayerInfos.Count))
        {
            State.TimeUntilRouteToGameMillis -= tickPeriod.TotalMilliseconds;
            if (State.TimeUntilRouteToGameMillis <= 0)
            {
                var gameId = State.GameId;
                var type = State.GameType;
                var userIds = State.PlayerInfos.Select(pi => pi.UserId).ToList();
                var gameGrain = grainFactory.GetGameGrain(gameId, type);
                await gameGrain.Initialize(gameId, State.PlayerInfos);
                await gameGrain.Start();
                foreach (var userId in userIds) await mediator.Publish(new UserJoinedGame(userId, gameId, type));
                await End();
            }

            await BroadcastRoomState();
        }
        else if (DateTime.UtcNow.Subtract(State.CreatedAt).TotalMilliseconds > StopAfter.TotalMilliseconds)
        {
            await End();
        }
        else if (Math.Abs(State.TimeUntilRouteToGameMillis - TimeUntilRouteToGame.TotalMilliseconds) > 2)
        {
            State.TimeUntilRouteToGameMillis = TimeUntilRouteToGame.TotalMilliseconds;
            await BroadcastRoomState();
        }


        await WriteStateAsync();
    }

    public async Task<(bool success, string message)> Join(UserId userId)
    {
        if (State.PlayerInfos.Count(pi => pi.UserId.Equals(userId)) == 0)
        {
            if (State.PlayerInfos.Count >= ViablePlayerCount.Max())
                return (success: false, message: "This room has maximum amount of players already :(");
            var playerInfo = await mediator.Send(new GetPlayersInfo(new List<UserId> { userId }));
            State.PlayerInfos.Add(playerInfo[0]);
        }

        await WriteStateAsync();
        await BroadcastRoomState();
        return (success: true, message: "Successfully joined the room!");
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        RoomStatus = RoomStatus.NotInitialized;
        await WriteStateAsync();

        timer?.Dispose();
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    private async Task TickHandler(object arg)
    {
        await this.AsReference<IRoomGrain>().Tick();
    }
}

[ExportTsEnum(OutputDir = "domain")]
[GenerateSerializer]
public enum RoomStatus
{
    NotInitialized,
    InProgress
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class RoomState(TimeSpan timeUntilRouteToGame, RoomId roomId, GameId gameId, GameType gameType)
{
    [Id(0)] public GameId GameId { get; set; } = gameId;
    [Id(1)] public GameType GameType { get; set; } = gameType;
    [Id(2)] public List<PlayerInfo> PlayerInfos { get; set; } = new();
    [Id(3)] public HashSet<UserId> ReadyPlayers { get; set; } = new();
    [Id(4)] public double TimeUntilRouteToGameMillis { get; set; } = timeUntilRouteToGame.TotalMilliseconds;
    [Id(5)] public RoomId RoomId { get; set; } = roomId;
    [Id(6)] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}