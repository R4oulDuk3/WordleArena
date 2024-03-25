<script>

    import {gameClientStore} from "$lib/clients/gameclient.ts";

    let playersOnline = 0

    
    $: gameClientStore.subscribe(async client => {
        if (client) {
            client.getConnection().on("PlayerCountUpdate", val => {
                playersOnline = val
            })
        }
    })

</script>

<div class="card w-5/6 max-w-xl flex justify-center items-center mb-4 p-2">
    <body class="card-body">Around <b> {playersOnline} </b> players currently online</body>
</div>