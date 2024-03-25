<script lang="ts">
    import '../app.postcss';
    import {AppBar, AppShell, initializeStores, Modal, storePopup, Toast} from '@skeletonlabs/skeleton';
    import {arrow, computePosition, flip, offset, shift} from '@floating-ui/dom';
    import GameConnection from '$lib/components/core/GameConnection.svelte';
    import UserSessionRefresh from "$lib/components/core/UserSessionRefresh.svelte";
    import {page} from '$app/stores';
    import {userStore} from "$lib/firebase.ts";
    import {get} from "svelte/store";
    import {onMount} from "svelte";
    import {routeToLogin} from "$lib/routing.ts";
    import {sleep} from "$lib/util.ts";

    let playerInGame = false
    initializeStores();
    storePopup.set({computePosition, flip, shift, offset, arrow});
    $: playerInGame = ($page.url.pathname.toLowerCase().includes("tempo") && !$page.url.pathname.toLowerCase().includes("result")) || $page.url.pathname.toLowerCase().includes("practice")
    onMount(async () => {
        page.subscribe(async value => {
            await sleep(600)
            let user = get(userStore)
            if (user == null && value.url.pathname.length > 1) {
                routeToLogin()
            }

        })
    })
</script>

<GameConnection></GameConnection>
<UserSessionRefresh></UserSessionRefresh>

<Modal/>
<Toast/>
<!-- App Shell -->
<AppShell>
    <svelte:fragment slot="header">
        <!-- App Bar -->
        {#if !playerInGame}
            <AppBar>
                <svelte:fragment slot="lead">
                    <div class="flex items-center">
                        <span class="box-decoration-slice text-white font-bold text-xl px-2">
                            Wordle
                        </span>
                        <span class="box-decoration-slice text-white text-xl bg-gradient-to-r from-purple-600 to-blue-400 px-2">
                            Arena
                        </span>
                    </div>
                </svelte:fragment>
                <!--                <LightSwitch/>-->
            </AppBar>
        {/if}

    </svelte:fragment>

    <!-- Page Route Content -->
    <div class="container h-full mx-auto flex flex-col justify-center items-center">

        <div/>

        <slot/>


</AppShell>
