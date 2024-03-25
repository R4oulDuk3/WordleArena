<script lang="ts">
    import {gameClientStore} from "$lib/clients/gameclient.ts";
    import {globalSessionStore} from "$lib/stores/user-session.ts";

    let isSubscribed = false

    gameClientStore.subscribe(async (client) => {
        if (client == null) {
            isSubscribed = false
        } else if (!isSubscribed) {
            client.getConnection().on("UserSession", val => {
                globalSessionStore.setSession(val)
            })
            isSubscribed = true
        }
    })

</script>