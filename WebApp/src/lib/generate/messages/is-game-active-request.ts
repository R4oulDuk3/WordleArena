/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { IRequest } from "./i-request";
import { IBaseRequest } from "./i-base-request";
import { IMessage } from "./i-message";
import { GameId } from "../domain/game-id";
import { GameType } from "../domain/game-type";

export interface IsGameActiveRequest extends IRequest<boolean>, IBaseRequest, IMessage {
    gameId: GameId;
    gameType: GameType;
}
