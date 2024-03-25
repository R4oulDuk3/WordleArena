<script lang="ts">
    import {slide} from 'svelte/transition';
    import {page} from '$app/stores';
    import PraticeGameManager from "$lib/components/game/PraticeGameManager.svelte";

    let gameId: string;
    $: gameId = $page.params.gameId;

</script>

<div class="container flex flex-col h-full items-center justify-center mx-auto h-full"
     transition:slide={{ duration: 300 }}>
    <PraticeGameManager {gameId}></PraticeGameManager>
</div>