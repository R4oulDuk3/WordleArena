using WordleArena.Api.Hubs.Messages;
using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Grains;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<ApiResponse<object?>> UpdateRoomPlayerReadiness(RoomPlayerUpdateReadyRequest request)
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var roomGrain = factory.GetGrain<IRoomGrain>(request.RoomId.Id);
        var status = await roomGrain.GetRoomStatus();
        if (status != RoomStatus.InProgress) return new ApiResponse<object?>(ApiResponseStatusCode.NotFound, null);

        var (success, message) = await roomGrain.UpdatePlayerReadiness(userId, request.IsReady);
        if (success) return new ApiResponse<object?>(ApiResponseStatusCode.Ok, null, message);
        return new ApiResponse<object?>(ApiResponseStatusCode.Forbidden, null, message);
    }

    public async Task<ApiResponse<RoomId?>> CreateARoom()
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var roomId = RoomId.GenerateNewRoomId();
        var roomGrain = factory.GetGrain<IRoomGrain>(roomId.Id);
        await roomGrain.Start(roomId, GameType.Tempo);
        var (success, message) = await roomGrain.Join(userId);
        if (success)
            return new ApiResponse<RoomId?>(ApiResponseStatusCode.Ok, roomId, message);
        return new ApiResponse<RoomId?>(ApiResponseStatusCode.Forbidden, null, message);
    }

    public async Task<ApiResponse<object?>> LeaveRoom(RoomId roomId)
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var roomGrain = factory.GetGrain<IRoomGrain>(roomId.Id);
        var status = await roomGrain.GetRoomStatus();
        if (status != RoomStatus.InProgress) return new ApiResponse<object?>(ApiResponseStatusCode.NotFound, null);
        var (success, message) = await roomGrain.Leave(userId);
        if (success)
        {
            await mediator.Publish(new UserLeftRoom(userId, roomId));

            return new ApiResponse<object?>(ApiResponseStatusCode.Ok, null, message);
        }

        return new ApiResponse<object?>(ApiResponseStatusCode.Forbidden, null, message);
    }

    public async Task<ApiResponse<RoomState?>> GetRoomState(RoomId roomId)
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var roomGrain = factory.GetGrain<IRoomGrain>(roomId.Id);
        var status = await roomGrain.GetRoomStatus();
        if (status != RoomStatus.InProgress) return new ApiResponse<RoomState?>(ApiResponseStatusCode.NotFound, null);

        var isParticipant = await roomGrain.IsPlayerInRoom(userId);
        if (isParticipant)
        {
            var state = await roomGrain.GetRoomState();
            return new ApiResponse<RoomState?>(ApiResponseStatusCode.Ok, state);
        }

        return new ApiResponse<RoomState?>(ApiResponseStatusCode.Forbidden, null,
            "You are not a participant of a room");
    }

    public async Task<ApiResponse<object?>> JoinRoom(JoinRoomRequest request)
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var roomGrain = factory.GetGrain<IRoomGrain>(request.RoomId.Id);
        var status = await roomGrain.GetRoomStatus();
        if (status != RoomStatus.InProgress) return new ApiResponse<object>(ApiResponseStatusCode.NotFound, null);
        var (success, message) = await roomGrain.Join(userId);

        if (success)
        {
            await mediator.Publish(new UserJoinedRoom(userId, request.RoomId));
            return new ApiResponse<object>(ApiResponseStatusCode.Ok, null, message);
        }

        return new ApiResponse<object?>(ApiResponseStatusCode.Forbidden, null, message);
    }
}