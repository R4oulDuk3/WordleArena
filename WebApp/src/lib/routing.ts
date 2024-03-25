import {goto} from "$app/navigation";
import {GameType} from "$lib/generate/domain/game-type.ts";
import type {GameId} from "$lib/generate/domain/game-id.ts";
import type {RoomId} from "$lib/generate/domain/room-id.ts";

export function routeToMainMenu() {
    goto("/main-menu")
}

export function routeToLogin() {
    goto("/")
}


export function routeToPlay() {
    goto("/main-menu/play")
}

export function routeToRoom(roomId: RoomId) {
    goto(`/room/${roomId.id}`)
}

export function routeToMatchmaking(gameType: GameType) {
    goto(`/matchmaking/${gameType}`)
}

export function routeToGame(gameType: GameType, gameId: GameId) {
    goto(`/${GameType[gameType].toLowerCase()}/${gameId.id}`)
}

export function routeToGameResults(gameType: GameType, gameId: GameId) {
    goto(`/result/${GameType[gameType].toLowerCase()}/${gameId.id}`)
}

export function routeToMatchHistory() {
    goto(`/matchhistory`)
}