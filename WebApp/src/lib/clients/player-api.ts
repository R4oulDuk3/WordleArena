import {getGameClient} from "./gameclient";
import type {ApiResponse} from "$lib/generate/messages/api-response.ts";

const GET_USERNAME = 'GetUsername'

export async function getUsername(): Promise<ApiResponse<string | null>> {

    let gameClient = await getGameClient()
    return await gameClient.getConnection().invoke<ApiResponse<string | null>>(GET_USERNAME);

}

export async function updateUsername(newUsername: string) {

    let gameClient = await getGameClient()

    await gameClient.getConnection().invoke('UpdateUsername', newUsername);

}
