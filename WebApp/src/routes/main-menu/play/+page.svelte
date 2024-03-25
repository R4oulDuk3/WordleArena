<script lang="ts">
    import PlayMenu from "$lib/components/menu/PlayMenu.svelte";
    import {slide} from 'svelte/transition';
</script>

<div class="container flex flex-col h-full items-center justify-center mx-auto" transition:slide={{ duration: 300 }}>
    <PlayMenu></PlayMenu>
</div>
