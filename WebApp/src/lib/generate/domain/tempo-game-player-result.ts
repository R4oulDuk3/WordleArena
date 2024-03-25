/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UserId } from "./user-id";
import { TempoPlayerResultInfo } from "./tempo-player-result-info";
import { GameId } from "./game-id";

export interface TempoGamePlayerResult {
    userId: UserId;
    resultInfo: TempoPlayerResultInfo;
    gameId: GameId;
    finishedAt: Date;
}
