/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SharedPlayerState } from "./shared-player-state";
import { UserId } from "./user-id";
import { PlayerScores } from "./player-scores";
import { PlayerInfo } from "./player-info";
import { PlayerAppliedEffectsState } from "./player-applied-effects-state";

export interface TempoGameSharedPlayerState extends SharedPlayerState {
    eliminationThresholdPoints: number;
    eliminatedPlayers: UserId[];
    playerScores: PlayerScores[];
    round: number;
    timeUntilNextEliminationMillis: number;
    hasBegun: boolean;
    gameIsOver: boolean;
    gameOverTimestamp: Date;
    countDownUntilBegin: number;
    participants: PlayerInfo[];
    readyPlayers: UserId[];
    playerAppliedEffectStates: PlayerAppliedEffectsState[];
}
