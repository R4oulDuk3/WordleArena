import * as signalR from "@microsoft/signalr";
import {get, writable, type Writable} from 'svelte/store';
import {sleep} from "$lib/util.ts";

export const messageStore = writable([]);

export class GameClient {

    connection: signalR.HubConnection;

    constructor(JTWCredentials: string, endpoint: string) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(endpoint,
                {accessTokenFactory: () => JTWCredentials}
            )
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Error)
            .build();
    }

    async connect() {
        await this.connection.start();
        console.log('Connection started');
    }


    isConnected() {
        return this.connection.state === signalR.HubConnectionState.Connected
    }


    isReconnecting() {
        return this.connection.state === signalR.HubConnectionState.Reconnecting
    }

    isDisconnected() {
        return this.connection.state === signalR.HubConnectionState.Disconnected
    }

    disconnect() {
        this.connection.stop()
            .then(() => console.log('Connection closed'))
            .catch(err => console.log('Error while closing the connection', err));
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any

    getConnection() {
        return this.connection;
    }

}

interface GameClientStore extends Writable<GameClient | null> {
    connect: (jwtToken: string, hubUrl: string) => Promise<void>;
    disconnect: () => void;
}

function createGameClientStore(): GameClientStore {
    const {subscribe, set, update} = writable<GameClient | null>(null);

    const connect = async (jwtToken: string, hubUrl: string): Promise<void> => {
        const client = new GameClient(jwtToken, hubUrl);
        await client.connect();
        set(client);
    };

    const disconnect = (): void => {
        update(client => {
            client?.disconnect();
            return null;
        });
    };

    return {
        subscribe,
        set,         // Include set in the returned object
        update,      // Include update as well
        connect,
        disconnect
    };
}

export const gameClientStore = createGameClientStore();


export async function getGameClient(sleepTime = 50, retryCount = 100) {
    let client;
    for (let attempt = 0; attempt < retryCount; attempt++) {
        client = get(gameClientStore);
        if (client !== null) {
            return client;
        }
        await sleep(sleepTime);
    }
    throw new Error('Failed to obtain client after maximum retries');
}