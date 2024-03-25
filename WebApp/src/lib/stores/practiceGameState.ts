import type {Writable} from "svelte/store";
import {writable} from "svelte/store";
import type {PracticeGamePlayerState} from "$lib/generate/domain/practice-game-player-state.ts";


export interface PracticeGameUserStateStore extends Writable<PracticeGamePlayerState> {
    setValues: (state: PracticeGamePlayerState) => void
}

export function createPracticeGameUserStateStore(state: PracticeGamePlayerState): PracticeGameUserStateStore {
    const {subscribe, set, update} = writable(state);

    const setValues = (newState: PracticeGamePlayerState) => {
        update(state => {
            return newState
        })
    }

    return {
        subscribe,
        set,
        update,
        setValues
    }
}
