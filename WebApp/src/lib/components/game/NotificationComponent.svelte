<script lang="ts">
    import {slide} from 'svelte/transition';
    import type {Writable} from "svelte/store";

    export let messageStore: Writable<string>;
    $: message = $messageStore;

    // Determine which heading level to use based on message length
    $: headingLevel = message.length > 50 ? 'h4' : message.length > 20 ? 'h3' : 'h2';
</script>

<div class="card fixed top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2 z-50 py-2 px-4 rounded-lg shadow-lg text-center"
     in:slide={{ duration: 500 }}
     out:slide={{ duration: 500 }}>
    {#if headingLevel === 'h2'}
        <h2>{message}</h2>
    {:else if headingLevel === 'h3'}
        <h3>{message}</h3>
    {:else}
        <h4>{message}</h4>
    {/if}
</div>
