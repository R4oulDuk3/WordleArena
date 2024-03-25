/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { Effect } from "../domain/effect";
import { GameId } from "../domain/game-id";
import { GameType } from "../domain/game-type";

export interface ApplyEffectRequest {
    effect: Effect;
    gameId: GameId;
    gameType: GameType;
}
