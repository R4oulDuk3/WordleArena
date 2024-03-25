/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GameId } from "../domain/game-id";
import { GetTempoGameResultResponseCode } from "./get-tempo-game-result-response-code";
import { TempoGamePlayerResultDto } from "./tempo-game-player-result-dto";

export interface GetTempoGameResultResponse {
    gameId: GameId;
    code: GetTempoGameResultResponseCode;
    tempoGamePlayerResultDtos: TempoGamePlayerResultDto[];
}
