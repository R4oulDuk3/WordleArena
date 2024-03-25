<script lang="ts">
    import {page} from '$app/stores';
    import TempoGamePlayerResult from "$lib/components/game/TempoGamePlayerResult.svelte";
    import type {GameId} from "$lib/generate/domain/game-id.ts";

    let gameId: GameId;
    $: gameId = {id: $page.params.gameId};

</script>
<div class="container flex flex-col h-full items-center justify-center mx-auto h-full">
    <TempoGamePlayerResult {gameId}/>
</div>