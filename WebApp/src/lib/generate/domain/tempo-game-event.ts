/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GameEvent } from "./game-event";
import { TempoGameEventType } from "./tempo-game-event-type";
import { UserId } from "./user-id";

export interface TempoGameEvent extends GameEvent {
    message: string;
    eventType: TempoGameEventType;
    player: UserId;
}
