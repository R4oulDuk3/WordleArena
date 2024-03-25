import type {UserSession} from "$lib/generate/domain/user-session.ts";
import type {Writable} from "svelte/store";
import {writable} from "svelte/store";


export interface UserSessionStore extends Writable<UserSession | null> {
    setSession: (state: UserSession) => void
}

export function createPracticeGameUserStateStore(): UserSessionStore {
    const {subscribe, set, update} = writable<UserSession | null>(null);

    const setSession = (newState: UserSession) => {
        update(state => {
            return newState
        })
    }

    return {
        subscribe,
        set,
        update,
        setSession
    }
}

export const globalSessionStore = createPracticeGameUserStateStore();