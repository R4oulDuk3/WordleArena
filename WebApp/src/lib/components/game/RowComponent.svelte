<script lang="ts">
    import type {GridStateStore} from "$lib/stores/gridState.ts";
    import {scale} from 'svelte/transition';
    import {LetterState} from "$lib/generate/domain/letter-state.ts";
    import type {Letter} from "$lib/generate/domain/letter.ts";
    import {spin} from "$lib/util.ts";

    export let rowIndex: number;
    export let gridStateStore: GridStateStore;


    function getAnimation(node: any, params: { letter: Letter }) {
        switch (params.letter.state) {
            case LetterState.Unknown:
                return scale(node, {duration: 1000});
            case LetterState.Misplaced:
                return spin(node, {duration: 1000});
            case LetterState.CorrectlyPlaced:
                return scale(node, {duration: 1000});
            case LetterState.NotPresentInWord:
                return scale(node, {duration: 1000});
            default:
                return scale(node, {duration: 1000});
        }
    }

    $: currentRowLetters = $gridStateStore.rows[rowIndex].letters;
</script>

<style>
    .unknown {
        background-color: lightgray;
    }

    .misplaced {
        background-color: orange;
    }

    .correctly-placed {
        background-color: green;
    }

    .not-present {
        background-color: red;
    }

    :root {

        --card-size: 2.5rem;
    }

    @media (min-height: 851px) {
        :root {

            --card-size: 3rem;
        }
    }

    @media (min-height: 1301px) {
        :root {

            --card-size: 3.5rem;
        }
    }

    .card {
        width: var(--card-size);
        height: var(--card-size);
    }
</style>


<div class="flex justify-center gap-2">
    {#each currentRowLetters as letter, index}
        {#key letter.value + letter.state}
            <div in:getAnimation={{letter}}
                 class="w-10 h-10 flex card rounded-none justify-center items-center text-4xl
                        {letter.state === LetterState.Unknown ? 'variant-ghost' : ''}
                        {letter.state === LetterState.Misplaced ? 'variant-filled-warning' : ''}
                        {letter.state === LetterState.CorrectlyPlaced ? 'variant-filled-success' : ''}
                        {letter.state === LetterState.NotPresentInWord ? 'variant-filled' : ''}">
                {letter.value.toUpperCase()}
            </div>
        {/key}
    {/each}
</div>
