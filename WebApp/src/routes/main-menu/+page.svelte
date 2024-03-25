<script lang="ts">

    import MainMenu from '$lib/components/menu/MainMenu.svelte';
    import OnlinePlayerCount from '$lib/components/menu/OnlinePlayerCount.svelte';
    import {slide} from 'svelte/transition';
</script>

<div class="container flex flex-col h-full items-center justify-center mx-auto"
     transition:slide={{ duration: 500 }}>
    <OnlinePlayerCount/>
    <MainMenu></MainMenu>
</div>
