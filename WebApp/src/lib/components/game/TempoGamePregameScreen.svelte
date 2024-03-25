<script lang="ts">
    import type {TempoGameSharedPlayerStateStore} from "$lib/stores/tempo-game-state.ts";
    import type {TempoGameSharedPlayerState} from "$lib/generate/domain/tempo-game-shared-player-state.ts";
    import type {PlayerInfo} from "$lib/generate/domain/player-info.ts";
    import {ProgressBar} from "@skeletonlabs/skeleton";
    import {onDestroy, onMount} from "svelte";

    export let sharedStateStore: TempoGameSharedPlayerStateStore

    let sharedState: TempoGameSharedPlayerState
    let allPlayers: PlayerInfo[] = []
    let readyPlayers: PlayerInfo[] = []
    let allPlayersReady = false

    let currentIndex = 0;
    const dotStates = ['...', '.  ', '.. ', '...'];
    let currentDotState = dotStates[0];

    function updateDotState() {
        currentIndex = (currentIndex + 1) % dotStates.length;
        currentDotState = dotStates[currentIndex];
    }

    let interval: any

    $: sharedStateStore.subscribe(state => {
        if (state != null) {
            sharedState = state
            allPlayers = state.participants
            readyPlayers = state.participants.filter(val =>
                state.readyPlayers.some(readyPlayer => val.userId.id == readyPlayer.id)
            );
            allPlayersReady = readyPlayers.length == state.participants.length
        }
    })
    onMount(() => {
        interval = setInterval(updateDotState, 500);
    })

    onDestroy(() => {
        // Clear the interval when the component is destroyed
        clearInterval(interval);
    });
</script>

<div class="card p-2 variant-filled text-center m-4">
    {#if allPlayersReady}
        <h3 class="h3">Game staring in:</h3>
        <h1 class="h1 m-2">{Math.trunc(sharedState.countDownUntilBegin / 1000)}</h1>
    {:else }
        <h3 class="h3 m-2">Waiting for all players to join</h3>
        <div class="m-4 ml-2 mr-2">
            <ProgressBar/>
        </div>
    {/if}
</div>

<div class="card p-4 variant-ghost text-center m-4">
    <h1 class="h1 m-2">Players</h1>

    {#each allPlayers as player, i}
        <div class="card m-2">
            {#if readyPlayers.some(val => val.userId.id === player.userId.id)}
                <div class="p-4">
                    <h4 class="h5">{i + 1}. {player.playerName}</h4>
                </div>
            {:else }
                <div class="p-4">
                    <h4 class="h5">Waiting for player {currentDotState}</h4>
                </div>
            {/if}
        </div>
    {/each}

</div>