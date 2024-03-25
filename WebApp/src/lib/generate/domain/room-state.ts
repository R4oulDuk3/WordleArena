/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GameId } from "./game-id";
import { GameType } from "./game-type";
import { PlayerInfo } from "./player-info";
import { UserId } from "./user-id";
import { RoomId } from "./room-id";

export interface RoomState {
    gameId: GameId;
    gameType: GameType;
    playerInfos: PlayerInfo[];
    readyPlayers: UserId[];
    timeUntilRouteToGameMillis: number;
    roomId: RoomId;
    createdAt: Date;
}
