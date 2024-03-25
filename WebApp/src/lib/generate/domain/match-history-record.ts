/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UserId } from "./user-id";
import { GameId } from "./game-id";

export interface MatchHistoryRecord {
    userId: UserId;
    gameId: GameId;
    finishedAt: Date;
    place: number;
    score: number;
}
