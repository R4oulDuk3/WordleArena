import {getGameClient} from "$lib/clients/gameclient.ts";
import type {ApiResponse} from "$lib/generate/messages/api-response.ts";
import type {MatchHistory} from "$lib/generate/domain/match-history.ts";


export async function getMatchHistory(page: number): Promise<ApiResponse<MatchHistory | null>> {
    let client = await getGameClient();
    return await client.getConnection().invoke<ApiResponse<MatchHistory | null>>("GetMatchHistory", page);
}