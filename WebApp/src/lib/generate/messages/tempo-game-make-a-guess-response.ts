/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { TempoGamePlayerState } from "../domain/tempo-game-player-state";
import { TempoGameEvent } from "../domain/tempo-game-event";

export interface TempoGameMakeAGuessResponse {
    playerState: TempoGamePlayerState;
    gameEvent: TempoGameEvent;
}
