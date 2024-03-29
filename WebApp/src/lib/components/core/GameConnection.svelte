<script lang="ts">
    import {onDestroy, onMount} from 'svelte';
    import {userStore} from '$lib/firebase';
    import type {User} from "firebase/auth";
    import {ProgressRadial} from '@skeletonlabs/skeleton';
    import {GameClient, gameClientStore} from '$lib/clients/gameclient'; // Assuming this is the correct path
    import {dev} from '$app/environment';

    let checkConnectionInterval: ReturnType<typeof setInterval> | null = null;
    let isCheckingConnection = false;
    const hubUrl: string = dev ? 'http://localhost:80/signalr' : 'http://localhost:80/signalr';
    let isLoggedIn = false
    let isConnectingFlag = false;
    let gameClient: GameClient | null = null;


    // Function to set the connecting status
    const setConnecting = (value: boolean): void => {
        isConnectingFlag = value;
    };

    // Connect the GameClient
    const connectGameClient = async (user: User): Promise<void> => {
        setConnecting(true);
        if (!gameClient) {
            console.log("Initializing new GameClient...");
            const jwtToken: string = await user.getIdToken(true);
            gameClient = new GameClient(jwtToken, hubUrl);
            await gameClient.connect()
            gameClientStore.set(gameClient);
        }
        if (gameClient.isReconnecting()) {
            console.log("GameClient is Reconnecting ...");
            return
        } else if (!gameClient.isConnected()) {
            console.log("GameClient disconnected. Attempting to reconnect...");
            await gameClient.connect();
            console.log("GameClient connected.");
        }
        setConnecting(false);
    };

    // Disconnect the GameClient
    const disconnectGameClient = (): void => {
        if (gameClient) {
            gameClient.disconnect();
            gameClient = null;
            gameClientStore.set(gameClient);
            console.log("GameClient disconnected.");
        }
        setConnecting(true);
    };

    // Check and handle the connection
    const checkAndHandleConnection = async (user: User | null): Promise<void> => {
        if (isCheckingConnection) {
            console.log("Connection check is already in progress, skipping...");
            return;
        }

        isCheckingConnection = true;

        try {
            if (user) {

                await connectGameClient(user);
            } else {
                console.log("No user detected. Disconnecting GameClient if exists...");
                disconnectGameClient();
            }
        } catch (e) {
            console.log("Error on checking and handling connection", e)
        }

        isCheckingConnection = false;
    };

    // Subscribe to user store and handle connection changes
    $: userStore.subscribe((user: User | null) => {
        isLoggedIn = user != null
        checkAndHandleConnection(user);
    });

    onMount(() => {
        console.log("Game Connection Component mounted.");
        checkConnectionInterval = setInterval(async () => {
            const user: User | null = $userStore;
            await checkAndHandleConnection(user);
        }, 1000);
    });

    onDestroy(() => {
        console.log("Game Connection Component being destroyed. Cleaning up...");
        if (checkConnectionInterval) clearInterval(checkConnectionInterval);
        disconnectGameClient();
    });
</script>

{#if isLoggedIn && isConnectingFlag}
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div class="card">
            <header class="card-header flex flex-col justify-center items-center">
                <h3 class="h3">Connecting to the server</h3>
            </header>
            <div class="flex flex-col justify-center items-center m-5">
                <ProgressRadial class="w-10"/>
            </div>
        </div>
    </div>
{/if}
