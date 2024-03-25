<script lang="ts">

    import type {GridStateStore} from "$lib/stores/gridState.ts"
    import {newLetter} from "$lib/stores/gridState.ts";

    const rows = [
        ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'],
        ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L'],
        ['Enter', 'Z', 'X', 'C', 'V', 'B', 'N', 'M', 'Del'],
    ];

    let buttonSize = "2rem"

    // export let letterStreamStore: LetterStreamStore
    export let gridStateStore: GridStateStore;
    export let disabled = true
    export let keyboardHints: string[] = []

    function handleKeyboardEvent(event: KeyboardEvent) {
        const key = event.key;
        handleKeyboardInput(key)
    }

    function handleKeyboardInput(key: string) {
        console.log("Input key", key);
        if (disabled) {
            return
        }
        if (key === "Delete" || key === "Del" || key === "Backspace") {
            gridStateStore.deleteLetter();
        } else if (key === "Enter") {
            gridStateStore.tryCurrentRowInputFinished()
        } else {
            const [isValid, letter] = newLetter(key);

            if (isValid && letter != null) {
                gridStateStore.addLetter(letter);
            }
        }
    }

</script>

<svelte:window on:keydown|preventDefault={handleKeyboardEvent}></svelte:window>

<div class="card m-2 p-2 rounded-sm variant-ghost-surface">
    {#each rows as row}
        <div class="container flex h-full items-center justify-center mx-auto">
            {#each row as letter}
                {#if keyboardHints.length === 0}
                    <button disabled={disabled}
                            class="btn btn-sm m-0.5 rounded-sm variant-filled pl-2 pr-2 pb-3 pt-3"
                            style="min-width: {buttonSize}"
                            on:click={() => handleKeyboardInput(letter)}>{letter}</button>
                {:else }
                    <button
                            class="btn btn-sm m-0.5 rounded-sm pl-2 pr-2 pb-3 pt-3 {!keyboardHints.includes(letter) ? 'variant-ghost' : 'variant-filled-success' }"
                            style="min-width: {buttonSize}"
                            on:click={() => handleKeyboardInput(letter)}>{letter}</button>
                {/if}

            {/each}
        </div>
    {/each}
</div>