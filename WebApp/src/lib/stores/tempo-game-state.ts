import type {Writable} from "svelte/store"
import {writable} from "svelte/store";
import type {TempoGamePlayerState} from "$lib/generate/domain/tempo-game-player-state.ts";
import type {TempoGameSharedPlayerState} from "$lib/generate/domain/tempo-game-shared-player-state.ts";

export interface TempoGamePlayerStateStore extends Writable<TempoGamePlayerState> {
    setValues: (state: TempoGamePlayerState) => void
}

export function createTempoGamePlayerStateStore(state: TempoGamePlayerState): TempoGamePlayerStateStore {
    const {subscribe, set, update} = writable(state);

    const setValues = (newState: TempoGamePlayerState) => {
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

export interface TempoGameSharedPlayerStateStore extends Writable<TempoGameSharedPlayerState | null> {
    setValue: (state: TempoGameSharedPlayerState) => void
}

export function createTempoGameSharedPlayerStateStore(state: TempoGameSharedPlayerState | null): TempoGameSharedPlayerStateStore {
    const {subscribe, set, update} = writable(state);

    const setValue = (newState: TempoGameSharedPlayerState) => {
        update(state => {
            return newState
        })
    }

    return {
        subscribe,
        set,
        update,
        setValue
    }
}