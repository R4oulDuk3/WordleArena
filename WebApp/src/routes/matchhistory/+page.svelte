<script lang="ts">

    import MatchHistory from "$lib/components/game/MatchHistory.svelte";
    import {globalSessionStore} from "$lib/stores/user-session.ts";
    import type {UserId} from "$lib/generate/domain/user-id.ts";

    let userId: UserId | null = null
    globalSessionStore.subscribe(val => {
        if (val) {
            userId = val.userId
        }
    })

</script>

{#if userId == null}
    Not allowed
{:else }

    <MatchHistory userId={userId}/>
{/if}