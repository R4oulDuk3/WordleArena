<script lang="ts">
    import type {Writable} from "svelte/store";

    export let timeLeftInMillis: Writable<number | null>;
    let firstValue: number | null = null;
    let time = 0;
    let percentage = 100; // Initialize the percentage of the timer

    $: timeLeftInMillis.subscribe((value) => {
        if (value !== null) {
            if (firstValue === null) {
                firstValue = value; // Ensure firstValue is only set once
            }
            time = value;
            percentage = (time / firstValue) * 100;
            // console.log(percentage)
        }
    });
</script>

<style>

    .timer-line {
        width: 100%;
        height: var(--percentage, 100%);
        transition: height 0.5s ease-out;
    }
</style>

<div class="h-full w-2 flex items-center">
    {#if firstValue !== null}
        <div class="timer-line variant-filled" style="--percentage: {percentage}%"></div>
    {/if}
</div>
