<script lang="ts">
    import {page} from '$app/stores';
    import RoomComponent from "$lib/components/game/RoomComponent.svelte";

    let roomId: string;
    $: roomId = $page.params.roomId;

</script>

<RoomComponent roomId={{id: roomId}}></RoomComponent>