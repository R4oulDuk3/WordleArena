<script lang="ts">
    import Row from "./RowComponent.svelte";
    import type {GridStateStore} from "$lib/stores/gridState.ts";
    import {get} from 'svelte/store';

    export let gridStateStore: GridStateStore;
    

</script>

<div>
    {#each get(gridStateStore).rows as row, index}
        <div class="mt-2">
            <Row {gridStateStore} rowIndex={index}/>
        </div>
    {/each}
</div>
