import type {ApiResponse} from "$lib/generate/messages/api-response.ts";
import type {RoomId} from "$lib/generate/domain/room-id.ts";
import {getGameClient} from "$lib/clients/gameclient.ts";
import type {RoomPlayerUpdateReadyRequest} from "$lib/generate/messages/room-player-update-ready-request.ts";
import type {RoomState} from "$lib/generate/domain/room-state.ts";
import type {JoinRoomRequest} from "$lib/generate/messages/join-room-request.ts";


export async function CreateARoom(): Promise<ApiResponse<RoomId | null>> {
    let client = await getGameClient()
    return await client.getConnection().invoke<ApiResponse<RoomId | null>>("CreateARoom");
}

export async function updateRoomPlayerReadiness(request: RoomPlayerUpdateReadyRequest): Promise<ApiResponse<null>> {
    let client = await getGameClient();
    return await client.getConnection().invoke<ApiResponse<null>>("UpdateRoomPlayerReadiness", request);
}

// Leave an existing room
export async function leaveRoom(roomId: RoomId): Promise<ApiResponse<null>> {
    let client = await getGameClient();
    return await client.getConnection().invoke<ApiResponse<null>>("LeaveRoom", roomId);
}

// Get the current state of a room
export async function getRoomState(roomId: RoomId): Promise<ApiResponse<RoomState | null>> {
    let client = await getGameClient();
    return await client.getConnection().invoke<ApiResponse<RoomState | null>>("GetRoomState", roomId);
}

// Join an existing room
export async function joinRoom(request: JoinRoomRequest): Promise<ApiResponse<null>> {
    let client = await getGameClient();
    return await client.getConnection().invoke<ApiResponse<null>>("JoinRoom", request);
}