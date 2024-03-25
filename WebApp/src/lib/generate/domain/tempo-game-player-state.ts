/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { GamePlayerState } from "./game-player-state";
import { GuessResult } from "./guess-result";
import { Hint } from "./hint";

export interface TempoGamePlayerState extends GamePlayerState {
    currentWordGuessResults: GuessResult[];
    currentWordLenght: number;
    allowedGuesses: number;
    hints: Hint[];
    tempoGauge: number;
    keyboardHints: string[];
}
