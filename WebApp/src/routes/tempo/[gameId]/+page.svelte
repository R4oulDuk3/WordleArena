<script lang="ts">
    import {slide} from 'svelte/transition';
    import {page} from '$app/stores';
    import TempoGameManager from "$lib/components/game/TempoGameManager.svelte";


    let gameId: string;
    $: gameId = $page.params.gameId;


</script>

<div class="container flex flex-col h-full items-center justify-center mx-auto h-full"
     transition:slide={{ duration: 300 }}>
    <TempoGameManager {gameId}></TempoGameManager>
</div>