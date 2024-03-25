/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { ParticipatingGame } from "./participating-game";
import { UserId } from "./user-id";
import { UserState } from "./user-state";
import { RoomId } from "./room-id";

export interface UserSession {
    activeParticipatingGamesIds: ParticipatingGame[];
    version: number;
    userId: UserId;
    connectionId: string;
    hostIpAddress: string;
    isConnected: boolean;
    userState: UserState;
    roomId: RoomId;
}
