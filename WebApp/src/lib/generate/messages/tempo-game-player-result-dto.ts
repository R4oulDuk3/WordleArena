/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { UserId } from "../domain/user-id";
import { TempoPlayerResultInfo } from "../domain/tempo-player-result-info";
import { PlayerInfo } from "../domain/player-info";

export interface TempoGamePlayerResultDto {
    userId: UserId;
    resultInfo: TempoPlayerResultInfo;
    playerInfo: PlayerInfo;
}
