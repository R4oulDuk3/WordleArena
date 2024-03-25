<script lang="ts">

    import type {RoomId} from "$lib/generate/domain/room-id.ts";
    import {onDestroy, onMount} from "svelte";
    import {getRoomState, joinRoom, leaveRoom, updateRoomPlayerReadiness} from "$lib/clients/room-api.ts";
    import type {Writable} from "svelte/store";
    import {get, writable} from "svelte/store";
    import type {RoomState} from "$lib/generate/domain/room-state.ts";
    import {ApiResponseStatusCode} from "$lib/generate/messages/api-response-status-code.ts";
    import type {ModalSettings} from "@skeletonlabs/skeleton";
    import {clipboard, getModalStore, ProgressBar, ProgressRadial} from "@skeletonlabs/skeleton";
    import {routeToGame, routeToMainMenu} from "$lib/routing.ts";
    import type {PlayerInfo} from "$lib/generate/domain/player-info.ts";
    import type {UserId} from "$lib/generate/domain/user-id.ts";
    import {globalSessionStore} from "$lib/stores/user-session.ts";
    import {getGameClient} from "$lib/clients/gameclient.ts";
    import {GameType} from "$lib/generate/domain/game-type.ts";

    const modalStore = getModalStore();

    export let roomId: RoomId
    let roomState: Writable<RoomState | null> = writable(null)
    let loading = false
    let roomNotFound = false
    let getStateFailed = false
    let failedReason: null | string = null
    let codeCopied = false
    let playerInfos: PlayerInfo [] = []
    let readyPlayers: UserId[] = []
    let playerReady = false
    let allPlayersReady = false
    let countUntilRouteToGame = 0;
    const modal: ModalSettings = {
        type: 'confirm',
        title: '',
        body: 'Are you sure you want to leave the Room?',
        response: async (r: boolean) => {
            if (r) {
                await leaveRoom(roomId);
                routeToMainMenu()
            }
        }
    };
    onMount(async () => {
        loading = true
        await joinRoom({roomId})
        try {
            let res = await getRoomState(roomId)
            if (res.statusCode == ApiResponseStatusCode.Ok) {
                roomState.set(res.data)
            } else if (res.statusCode == ApiResponseStatusCode.NotFound) {
                roomNotFound = true
            } else {
                getStateFailed = true
                failedReason = res.message
            }
            let client = await getGameClient()
            client.getConnection().on(`roomState:${roomId.id}`, (val: RoomState) => {
                console.log("Received roomState", val)
                roomState.set(val)
            })
        } catch (e) {
            roomNotFound = true
        }
        loading = false
    })

    onDestroy(async () => {
        let client = await getGameClient()
        client.getConnection().off(`roomState:${roomId.id}`)
    })

    roomState.subscribe(async state => {
        if (state != null) {
            playerInfos = state.playerInfos
            readyPlayers = state.readyPlayers
            playerReady = state.readyPlayers.some(p => p.id == get(globalSessionStore)?.userId.id)
            allPlayersReady = state.readyPlayers.length == state.readyPlayers.length && (state.readyPlayers.length == 2 || state.readyPlayers.length == 4)
            countUntilRouteToGame = state.timeUntilRouteToGameMillis
            if (countUntilRouteToGame <= 0) {
                let client = await getGameClient()
                client.getConnection().off("roomState")
                routeToGame(GameType.Tempo, state.gameId)
            }
        }
    })


    function copyCodeAndReset() {
        codeCopied = true;

        setTimeout(() => {
            codeCopied = false;
        }, 1000);
    }

    async function promptLeaveRoom() {
        modalStore.trigger(modal)
    }

    async function updateReadinessState() {
        console.log("Update")
        await updateRoomPlayerReadiness({roomId, isReady: !playerReady})
        let res = await getRoomState(roomId)
        roomState.set(res.data)
    }


</script>

{#if loading}
    <ProgressRadial/>
{:else if roomNotFound}

    <div class="card">
        <header class="card-header">Ug-Oh</header>
        <section class="p-4">We could not find the room you were looking for :(</section>
        <button type="button" class="btn btn-lg variant-filled" on:click={routeToMainMenu}>Return to menu</button>
    </div>

{:else if getStateFailed}
    <div class="card">
        <header class="card-header">Ug-Oh</header>
        <section class="p-4">You could not enter the room: {failedReason}</section>
        <button type="button" class="btn btn-lg variant-filled" on:click={routeToMainMenu}>Return to menu</button>
    </div>

{:else }
    <div class="h-full flex flex-col justify-between">
        <div>
            <div class="card p-2 variant-filled text-center m-4">
                {#if allPlayersReady}
                    <h3 class="h3">Game staring in:</h3>
                    <h1 class="h1 m-2">{Math.trunc(countUntilRouteToGame / 1000)}</h1>
                {:else }
                    <h3 class="h3 m-2">Waiting for players</h3>
                    <div class="m-4 ml-2 mr-2">
                        <ProgressBar/>
                    </div>
                {/if}
            </div>
            <div class="card variant-ghost">
                <header class="card-header">
                    <h3 class="h3">Players ({`${playerInfos.length}/4`})</h3>
                    <p>2 or 4 players are required for a game</p>
                </header>
                <section class="p-4">
                    {#each playerInfos as player, i}
                        <div class="card">
                            <div class="p-4 flex justify-between">
                                <h5 class="h5">{i + 1}. {player.playerName}</h5>
                                <h5 class="ml-1 h5 {readyPlayers.some(val => val.id === player.userId.id) ? '' : 'invisible'}">
                                    Ready!
                                </h5>
                            </div>
                        </div>
                    {/each}
                </section>
                <footer class="card-footer flex justify-around">
                    <button type="button" class="btn btn-lg variant-filled" on:click={promptLeaveRoom}>Leave</button>
                    <button type="button"
                            class="btn btn-lg variant-filled {playerReady ? 'variant-filled-success' : ''}"
                            on:click={updateReadinessState}>
                        {playerReady ? "Ready!" : "Ready?"}
                    </button>
                </footer>
            </div>
        </div>
        <div class="card p-3 mb-4 variant-filled text-center">
            <section class="flex flex-col justify-center items-center">
                <h3 class="h3 p-1">
                    Room Code
                </h3>

                <h2 class="h2">
                    {roomId.id}
                </h2>
                <button class="btn variant-ghost" on:click={copyCodeAndReset}
                        use:clipboard={roomId.id}>{!codeCopied ? "Copy" : "Copied!"}</button>
            </section>
        </div>
    </div>
{/if}


