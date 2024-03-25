<script lang="ts">
    import type {GameType} from "$lib/generate/domain/game-type.ts";
    import {ProgressRadial} from "@skeletonlabs/skeleton";

    import {onMount} from "svelte";
    import {TryJoinMatchmaking} from "$lib/clients/matchmaking-client.ts";
    import {globalSessionStore} from "$lib/stores/user-session.ts";
    import {routeToGame} from "$lib/routing.ts";
    import type {ParticipatingGame} from "$lib/generate/domain/participating-game.ts";

    onMount(async () => {
        await TryJoinMatchmaking(gameType)
    })

    $: globalSessionStore.subscribe(async session => {
        if (session != null) {
            // console.log(session)
            if (session.activeParticipatingGamesIds.filter(v => v.gameType == gameType).length == 1) {
                const participatingGame: ParticipatingGame = session.activeParticipatingGamesIds.filter(v => v.gameType == gameType)[0];
                console.log("Cool", participatingGame)
                routeToGame(gameType, participatingGame.gameId)

            }
        }
    })

    export let gameType: GameType

</script>

<div>
    <ProgressRadial/>
</div>