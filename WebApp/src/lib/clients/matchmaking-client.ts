import type {GameType} from "$lib/generate/domain/game-type.ts";
import {getGameClient} from "$lib/clients/gameclient.ts";
import type {JoinMatchmakingRequest} from "$lib/generate/messages/join-matchmaking-request.ts";


const TryJoinMatchmaking_method = "TryJoinMatchmaking"
const LeaveMatchmaking_method = "LeaveMatchmaking"

export async function TryJoinMatchmaking(gameType: GameType) {
    let client = await getGameClient()
    let joinMatchmakingRequest: JoinMatchmakingRequest = {gameType}
    await client.getConnection().invoke(TryJoinMatchmaking_method, joinMatchmakingRequest)
}

export async function LeaveMatchmaking() {
    let client = await getGameClient()
    await client.getConnection().invoke(LeaveMatchmaking_method)
}