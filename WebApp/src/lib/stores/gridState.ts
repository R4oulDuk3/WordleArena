import type {Writable} from 'svelte/store';
import {writable} from "svelte/store";
import {LetterState} from "$lib/generate/domain/letter-state.ts";
import type {Letter} from "$lib/generate/domain/letter.ts";
import type {GuessResult} from "$lib/generate/domain/guess-result.ts";

export class Word {
    value: string;

    constructor(row: Row) {
        this.value = row.letters.map(letter => letter.value).join('');
    }
}


function createEmptyLetter(): Letter {
    return {
        value: " ",
        state: LetterState.Unknown
    }
}

export function newLetter(input: string): [boolean, Letter | null] {
    const isValid = /^[a-zA-Z]$/.test(input) && input.length == 1;
    if (isValid) {
        return [true, {value: input.toUpperCase(), state: LetterState.Unknown}];
    } else {
        return [false, null];
    }
}

function isLetterEmpty(letter: Letter): boolean {
    return letter.value === " " || letter.value === "";
}

function clone(letter: Letter): Letter {
    return {
        value: letter.value,
        state: letter.state
    }
}

export class Row {

    wordLength: number
    letters: Letter[]
    currentLetterIndex: number

    constructor(wordLength: number, guessResult: GuessResult | null = null) {
        this.letters = Array(wordLength).fill(createEmptyLetter());
        if (guessResult) {
            toMap(guessResult.letterByPosition).forEach((value, key) => {
                this.letters[key] = value
            })
        }
        this.currentLetterIndex = 0;
        this.wordLength = wordLength;
    }

    isFull(): boolean {
        return this.letters.every(letter => !isLetterEmpty(letter))
    }

    isEmpty(): boolean {
        return this.letters.every(letter => isLetterEmpty(letter))
    }

    addLetter(letter: Letter) {
        if (this.isFull()) {
            throw new Error("Game Logic error, row is full!");
        }
        this.letters[this.currentLetterIndex] = letter;
        this.currentLetterIndex++;
    }

    deleteLetter() {
        if (this.isEmpty()) {
            throw new Error("Game Logic error, row is empty!");
        }
        this.currentLetterIndex--;
        this.letters[this.currentLetterIndex] = createEmptyLetter();
    }

    clone(): Row {
        const newRow = new Row(this.wordLength);
        newRow.letters = this.letters.map(letter => clone(letter));
        newRow.currentLetterIndex = this.currentLetterIndex;
        return newRow;
    }

    updateLetterState(guessResult: GuessResult): void {
        console.log("map: ", guessResult)
        console.log(typeof guessResult.letterByPosition);

        for (const [key, value] of toMap(guessResult.letterByPosition)) {
            this.letters[key] = value;
        }
    }

    getWord(): Word {
        return new Word(this)
    }
}

function toMap(letterByPosition: { [key: number]: Letter; }) {
    let res = new Map<number, Letter>;
    (new Map(Object.entries(letterByPosition)).forEach((val, key) => res.set(Number(key), val)))
    return res
}

export class GridState {

    wordLength: number
    rows: Row[]
    currentRow: number
    currentRowSubmitted: boolean

    constructor(wordLength: number, rowsCount: number, guessResults: GuessResult[] = []) {
        this.wordLength = wordLength;
        this.rows = [...guessResults.map(result => new Row(wordLength, result)),
            ...Array.from({length: rowsCount - guessResults.length}, () => new Row(wordLength))];
        this.currentRow = guessResults.length;
        this.currentRowSubmitted = false;
    }

    addLetter(letter: Letter) {
        console.log("Current row: ", this.currentRow)
        let row = this.rows[this.currentRow]
        if (row.isFull()) {
            return
        }
        row.addLetter(letter)
    }

    tryCurrentRowInputFinished() {
        let row = this.rows[this.currentRow]
        if (!row.isFull()) {
            return
        }
        this.currentRowSubmitted = true
    }

    unlockGrid() {
        this.currentRowSubmitted = false
    }

    deleteLetter() {
        let row = this.rows[this.currentRow]
        if (row.isEmpty()) {
            return
        }
        row.deleteLetter()
    }

    clone(): GridState {
        const newGridState = new GridState(this.wordLength, this.rows.length);
        newGridState.rows = this.rows.map(row => row.clone());
        newGridState.currentRow = this.currentRow;
        newGridState.currentRowSubmitted = this.currentRowSubmitted
        return newGridState;
    }

    updateLetterState(letterStatesPerRow: Map<number, GuessResult>): void {
        let rowIndex = 0
        letterStatesPerRow.forEach((value, key) => {
            let row = this.rows[key]
            row.updateLetterState(value)
            rowIndex++;
        })
        this.currentRow = rowIndex
    }

    getCurrentWord(): Word {
        let row = this.rows[this.currentRow]
        return row.getWord()
    }

}

export interface GridStateStore extends Writable<GridState> {
    reset: (wordLength: number, rowCount: number) => void;
    addLetter: (letter: Letter) => void
    deleteLetter: () => void
    updateLetterState: (mapping: Map<number, GuessResult>) => void
    tryCurrentRowInputFinished: () => void
    unlockGrid: () => void
}

export function createGirdStateStore(wordLength: number, rowCount: number, guessResults: GuessResult[] = []): GridStateStore {
    const initialState = new GridState(wordLength, rowCount, guessResults);
    const {subscribe, set, update} = writable(initialState);

    const reset = (wordLength: number, rowCount: number) => {
        set(new GridState(wordLength, rowCount));
    }

    const addLetter = (letter: Letter) => {
        update(state => {
            if (state.currentRowSubmitted) {
                return state
            }
            state.addLetter(letter);
            return state.clone();
        });
    }

    const deleteLetter = () => {
        update(state => {
            if (state.currentRowSubmitted) {
                return state
            }
            state.deleteLetter();
            return state.clone()
        })
    }

    const updateLetterState = (mapping: Map<number, GuessResult>) => {
        update(state => {
            state.updateLetterState(mapping);
            return state.clone()
        })
    }

    const tryCurrentRowInputFinished = () => {
        update(state => {

                state.tryCurrentRowInputFinished();
                return state.clone()
            }
        )
    }

    const unlockGrid = () => {
        update(state => {
                state.unlockGrid();
                return state.clone()
            }
        )
    }

    return {
        subscribe,
        set,
        update,
        reset,
        addLetter,
        deleteLetter,
        updateLetterState,
        tryCurrentRowInputFinished,
        unlockGrid
    }
}

