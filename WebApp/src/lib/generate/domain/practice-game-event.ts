/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GameEvent } from "./game-event";
import { PracticeGameEventType } from "./practice-game-event-type";
import { UserId } from "./user-id";

export interface PracticeGameEvent extends GameEvent {
    eventType: PracticeGameEventType;
    player: UserId;
    message: string;
}
