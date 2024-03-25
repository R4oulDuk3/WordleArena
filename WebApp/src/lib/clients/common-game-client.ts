import {getGameClient} from "$lib/clients/gameclient.ts";
import type {GameId} from "$lib/generate/domain/game-id.ts";
import type {SignalizeReadinessRequest} from "$lib/generate/messages/signalize-readiness-request.ts";
import type {GameType} from "$lib/generate/domain/game-type.ts";
import type {GetPlayerStateRequest} from "$lib/generate/messages/get-player-state-request.ts";
import type {Guess} from "$lib/generate/domain/guess.ts";
import type {MakeAGuessRequest} from "$lib/generate/messages/make-a-guess-request.ts";
import type {GoToNextWordRequest} from "$lib/generate/messages/go-to-next-word-request.ts";
import type {PracticeGameMakeAGuessResponse} from "$lib/generate/messages/practice-game-make-a-guess-response.ts";
import type {TempoGameMakeAGuessResponse} from "$lib/generate/messages/tempo-game-make-a-guess-response.ts";
import type {EffectType} from "$lib/generate/domain/effect-type.ts";
import type {ApplyEffectRequest} from "$lib/generate/messages/apply-effect-request.ts";
import type {Effect} from "$lib/generate/domain/effect.ts";
import type {UserId} from "$lib/generate/domain/user-id.ts";
import type {IsGameActiveRequest} from "$lib/generate/messages/is-game-active-request.ts";
import type {GameStatus} from "$lib/generate/domain/game-status.ts";
import type {SignalizeReadinessResponse} from "$lib/generate/domain/signalize-readiness-response.ts";
import type {GetTempoGamePlayerResultRequest} from "$lib/generate/messages/get-tempo-game-player-result-request.ts";
import type {GetTempoGameResultResponse} from "$lib/generate/messages/get-tempo-game-result-response.ts";


export async function signalizeReadiness(gameId: GameId, gameType: GameType): Promise<SignalizeReadinessResponse> {

    let gameClient = await getGameClient()
    let req: SignalizeReadinessRequest = {
        gameId, gameType
    }
    return await gameClient.getConnection().invoke<SignalizeReadinessResponse>("SignalizeReadiness", req);
}

const getPlayerState_method = "GetGamePlayerState"
const PracticeGameMakeAGuess_method = "PracticeGameMakeAGuess"
const TempoGameMakeAGuess_method = "TempoGameMakeAGuess"
const GoToNextWord_method = "GoToNextWord"
const ApplyEffect_method = "ApplyEffect"

export async function GetPlayerState<T>(gameId: GameId, gameType: GameType): Promise<T> {
    let gameClient = await getGameClient()
    let request: GetPlayerStateRequest = {gameId, gameType}
    return await gameClient.getConnection().invoke<T>(getPlayerState_method, request)
}

export async function PracticeGameMakeAGuess(
    gameId: GameId,
    guess: Guess
) {
    let client = await getGameClient();
    let request: MakeAGuessRequest = {gameId, guess};
    return await client.getConnection().invoke<PracticeGameMakeAGuessResponse>(PracticeGameMakeAGuess_method, request);
}

export async function TempoGameMakeAGuess(
    gameId: GameId,
    guess: Guess
) {
    let client = await getGameClient();
    let request: MakeAGuessRequest = {gameId, guess};
    return await client.getConnection().invoke<TempoGameMakeAGuessResponse>(TempoGameMakeAGuess_method, request);
}

export async function GoToNextWord<T>(gameId: GameId, gameType: GameType) {
    try {
        let client = await getGameClient()
        let request: GoToNextWordRequest = {gameId, gameType}
        return await client.getConnection().invoke<T>(GoToNextWord_method, request)
    } catch (e) {
        console.error("An error occurred on MakeAGuess", e)
        throw e;
    }
}

export async function ApplyEffect<T>(gameId: GameId, gameType: GameType, targetPlayer: UserId, type: EffectType) {
    let client = await getGameClient()
    let effect: Effect = {type, targetPlayer}
    let request: ApplyEffectRequest = {gameId, effect, gameType}
    return await client.getConnection().invoke<T>(ApplyEffect_method, request)
}

export async function GetGameStatus(gameId: GameId, gameType: GameType) {
    let client = await getGameClient()
    let request: IsGameActiveRequest = {gameId, gameType}
    return await client.getConnection().invoke<GameStatus>("GetGameStatus", request)
}

export async function GetTempoGameResult(gameId: GameId) {
    let client = await getGameClient()
    let req: GetTempoGamePlayerResultRequest = {gameId}
    return await client.getConnection().invoke<GetTempoGameResultResponse>("GetTempoGameResult", req)
}