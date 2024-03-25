<script lang="ts">
    import type {GameType} from "$lib/generate/domain/game-type.ts";
    import {page} from "$app/stores";
    import {slide} from 'svelte/transition';
    import MatchmakingScreen from "$lib/components/matchmaking/MatchmakingScreen.svelte";

    let gameType: GameType;
    $:gameType = Number($page.params.gameType);


</script>

<div class="container flex flex-col h-full items-center justify-center mx-auto" transition:slide={{ duration: 300 }}>
    <MatchmakingScreen {gameType}></MatchmakingScreen>
</div>
