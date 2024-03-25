<script lang="ts">
    import {routeToMainMenu, routeToMatchmaking, routeToRoom} from "$lib/routing";
    import {GameType} from "$lib/generate/domain/game-type";
    import {CreateARoom, joinRoom} from "$lib/clients/room-api";
    import {getToastStore} from '@skeletonlabs/skeleton';
    import {ApiResponseStatusCode} from "$lib/generate/messages/api-response-status-code";
    import type {RoomId} from "$lib/generate/domain/room-id.ts";
    import {onMount} from "svelte";
    import {getUsername, updateUsername} from "$lib/clients/player-api.ts";

    const toastStore = getToastStore();

    async function tryCreateARoom() {
        let response = await CreateARoom()
        if (response.statusCode === ApiResponseStatusCode.Ok && response.data != null) {
            routeToRoom(response.data)
        } else {
            const t = {
                message: 'Could not create a room at the moment, try again later...',
            };
            toastStore.trigger(t);
        }
    }

    let joinRoomId = ""

    async function tryJoinTheRoom() {
        let roomId: RoomId = {id: joinRoomId}
        console.log("Trying to join room", roomId)
        let res = await joinRoom({roomId})
        if (res.statusCode == ApiResponseStatusCode.Ok) {
            routeToRoom(roomId)
        } else if (res.statusCode == ApiResponseStatusCode.NotFound) {
            const t = {
                message: 'Could not find the room, check your room Id',
            };
            toastStore.trigger(t);
        } else {
            const t = {
                message: res.message,
            };
            toastStore.trigger(t);
        }
    }

    let username = ""

    onMount(async () => {
        username = (await getUsername()).data ?? "Unknown"
    })

    async function tryUpdateName() {
        try {
            await updateUsername(username)
            const t = {
                message: 'Name successfully updated',
            };
            toastStore.trigger(t);
        } catch (e) {
            const t = {
                message: 'Could not update name, try again later :(',
            };
            toastStore.trigger(t);
        }
    }
</script>

<div class="card w-5/6 max-w-xl">
    <header class="card-header flex justify-between items-center text-center">
        <button class="btn variant-ghost" on:click={routeToMainMenu} type="button">{"<"}</button>
        <h3 class="h3">Lets find a game!</h3>
        <div></div>
    </header>
    <section class="p-4 flex flex-col justify-center items-center">
        <label class="label w-3/4">
            <span>Give your self a cool name :)</span>
            <div class="flex">
                <input bind:value={username} class="input" maxlength="15" placeholder={username} type="text"/>
                <button class="ml-1 btn variant-ghost" on:click={tryUpdateName} type="button"> ></button>
            </div>
        </label>
        <div class="flex w-full items-center my-3">
            <div class="flex-grow border-t variant-filled"></div>
        </div>
        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={async () => { routeToMatchmaking(GameType.Tempo)}} type="button">
            <span>Find a match</span>
        </button>
        <div class="mb-3 w-3/4 flex">
            <label class="label w-full">
                <input bind:value={joinRoomId} class="input" maxlength="6" placeholder="Room id" type="text"/>
            </label>
            <button class="ml-1 btn variant-ghost" on:click={tryJoinTheRoom} type="button"> ></button>
        </div>
        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={tryCreateARoom}
                type="button">
            <span>Create a Room</span>
        </button>


    </section>


</div>
				