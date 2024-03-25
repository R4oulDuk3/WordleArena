/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GameId } from "../domain/game-id";
import { Guess } from "../domain/guess";

export interface MakeAGuessRequest {
    gameId: GameId;
    guess: Guess;
}
