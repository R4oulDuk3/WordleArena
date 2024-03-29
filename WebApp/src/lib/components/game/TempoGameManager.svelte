<script lang="ts">
    import {createGirdStateStore} from "$lib/stores/gridState.ts";
    import type {TempoGamePlayerStateStore, TempoGameSharedPlayerStateStore} from "$lib/stores/tempo-game-state.ts";
    import {
        createTempoGamePlayerStateStore,
        createTempoGameSharedPlayerStateStore
    } from "$lib/stores/tempo-game-state.ts";
    import TempoGamePregameScreen from "$lib/components/game/TempoGamePregameScreen.svelte";
    import {onDestroy, onMount} from "svelte";
    import {getGameClient} from "$lib/clients/gameclient.ts";
    import type {TempoGameSharedPlayerState} from "$lib/generate/domain/tempo-game-shared-player-state.ts";
    import PreGameLoadScreen from "$lib/components/game/PreGameLoadScreen.svelte";
    import {
        ApplyEffect,
        GetGameStatus,
        GetPlayerState,
        GoToNextWord,
        signalizeReadiness,
        TempoGameMakeAGuess
    } from "$lib/clients/common-game-client.ts";
    import {GameType} from "$lib/generate/domain/game-type.ts";
    import Keyboard from "$lib/components/game/Keyboard.svelte";
    import GridComponent from "$lib/components/game/GridComponent.svelte";
    import type {TempoGamePlayerState} from "$lib/generate/domain/tempo-game-player-state.ts";
    import type {GuessResult} from "$lib/generate/domain/guess-result.ts";
    import {cutNameLenght, sleep} from "$lib/util.ts";
    import type {TempoGameEvent} from "$lib/generate/domain/tempo-game-event.ts";
    import {TempoGameEventType} from "$lib/generate/domain/tempo-game-event-type.ts";
    import type {PlayerScores} from "$lib/generate/domain/player-scores.ts";
    import {globalSessionStore} from "$lib/stores/user-session.ts";
    import {get, writable} from "svelte/store";
    import type {UserId} from "$lib/generate/domain/user-id.ts";
    import type {Hint} from "$lib/generate/domain/hint.ts";
    import {slide} from 'svelte/transition';
    import {HintType} from "$lib/generate/domain/hint-type.ts";
    // import IconAccessibility from '~icons/carbon/accessibility';
    import StreamlineInterfaceWeatherSnowFlakeWinterFreezeSnowFreezingIceColdWeatherSnowflake
        from '~icons/streamline/interface-weather-snow-flake-winter-freeze-snow-freezing-ice-cold-weather-snowflake';
    import GameIconsShieldReflect from '~icons/game-icons/shield-reflect';
    import MaterialSymbolsKeyboardSharp from '~icons/material-symbols/keyboard-sharp';
    import {EffectType} from "$lib/generate/domain/effect-type.ts";
    import type {ModalSettings} from "@skeletonlabs/skeleton";
    import {getModalStore, ProgressBar} from "@skeletonlabs/skeleton";
    import type {PlayerAppliedEffect} from "$lib/generate/domain/player-applied-effect.ts";
    import NotificationComponent from "$lib/components/game/NotificationComponent.svelte";
    import type {InGameNotification} from "$lib/generate/domain/in-game-notification.ts";
    import MdiDeath from '~icons/mdi/death';
    import {routeToGameResults, routeToMainMenu} from "$lib/routing.ts";
    import {GameStatus} from "$lib/generate/domain/game-status.ts";

    const modalStore = getModalStore();
    const modal: ModalSettings = {
        type: 'confirm',
        title: '',
        body: 'Are you sure you want to leave the game?',
        response: async (r: boolean) => {
            await cleanup()
            if (r) routeToMainMenu()
        }
    };
    export let gameId: string
    let gridStateStore = createGirdStateStore(4, 5)
    let playerStateStore: TempoGamePlayerStateStore | null
    let sharedStateStore: TempoGameSharedPlayerStateStore = createTempoGameSharedPlayerStateStore(null)
    let processingGirdState = false
    let hasInitialized = false
    let gridRecreateTimestamp = 0
    let playerScores: PlayerScores[] = []
    let hasBegun = false;

    let notificationMessageStore = writable("Initial")
    let showNotificationStore = writable(false);
    let timeBetweenWordSwitch = 1000
    let timeUntilNextEliminationMillis = 0;
    let eliminationThreshold = 0;

    let hint: Hint | null = null;
    let applyingEffect = false
    let tempoGauge = 0
    let allPlayersAppliedEffects: PlayerAppliedEffect[]

    let isEliminated = false
    let effectFrozen = false
    let reflectApplied = false
    let enoughTempoForAbility = false
    let keyboardHints: string[] = []
    let effectTempoCost = 100;
    let eliminatedPlayers: UserId[] = []
    let getGameStatusStub: any

    async function initializeCallbacks() {
        let client = await getGameClient()

        client.getConnection().on("SendTempoSharedState", (val: TempoGameSharedPlayerState) => {
            sharedStateStore.setValue(val)
        })
        client.getConnection().on("SendInGameNotification", async (val: InGameNotification) => {
            while ($showNotificationStore) {
                await new Promise(resolve => setTimeout(resolve, 100));
            }
            notificationMessageStore.set(val.text)
            showNotif(val.showNotificationDuration)
        })

        client.getConnection().on("TempoGameEvent", async (val: TempoGameEvent) => {
            await handleGameEvent(val)
        })

        getGameStatusStub = setInterval(async () => {
            let status = await GetGameStatus({id: gameId}, GameType.Tempo);
            if (status == GameStatus.Finished) {
                routeToGameResults(GameType.Tempo, {id: gameId})
            }
            if (status == GameStatus.NotInitialized) {
                routeToMainMenu()
            }
        }, 1500);
    }


    async function refreshPlayerState() {
        let tempoGamePlayerState = await GetPlayerState<TempoGamePlayerState>({id: gameId}, GameType.Tempo);
        updateFromPlayerState(tempoGamePlayerState);
    }

    function updateFromPlayerState(tempoGamePlayerState: TempoGamePlayerState) {
        gridStateStore = createGirdStateStore(tempoGamePlayerState.currentWordLenght, tempoGamePlayerState.allowedGuesses)
        if (playerStateStore == null) {
            playerStateStore = createTempoGamePlayerStateStore(tempoGamePlayerState)
        } else {
            playerStateStore.set(tempoGamePlayerState)
        }
        gridRecreateTimestamp = Date.now()
    }

    $: playerStateStore?.subscribe(state => {
        let row = 0
        let map = new Map<number, GuessResult>()
        state.currentWordGuessResults.forEach(result => {
            map.set(row, result)
            row++;
        })
        gridStateStore.updateLetterState(map)
        hint = state.hints[state.hints.length - 1]
        tempoGauge = state.tempoGauge
        keyboardHints = state.keyboardHints
        enoughTempoForAbility = tempoGauge >= effectTempoCost
    })

    async function cleanup() {
        try {
            clearInterval(getGameStatusStub)
            let client = await getGameClient()
            client.getConnection().off("SendTempoSharedState")
            client.getConnection().off("SendInGameNotification")
            client.getConnection().off("TempoGameEvent")
        } catch (e) {
            console.error("Error on destroy", e)
        }
    }

    onDestroy(async () => {
        await cleanup();
    });

    $: sharedStateStore.subscribe(async state => {
        hasInitialized = state != null
        if (state != null) {
            if (state.gameIsOver) {
                console.log("Game over")
                routeToGameResults(GameType.Tempo, {id: gameId})
            }
            if (hasBegun == false && state.hasBegun) {
                console.log("Game has begun")
                await refreshPlayerState()
            }
            hasBegun = state.hasBegun;

            playerScores = state.playerScores.sort((a, b) => {
                if (a.userId.id === get(globalSessionStore)?.userId.id) return -1;
                if (b.userId.id === get(globalSessionStore)?.userId.id) return 1;
                return 0;
            });
            timeUntilNextEliminationMillis = state.timeUntilNextEliminationMillis
            eliminationThreshold = state.eliminationThresholdPoints
            allPlayersAppliedEffects = state.playerAppliedEffectStates
            let currentPlayerAppliedEffect = state.playerAppliedEffectStates.find(val => val.userId.id == get(globalSessionStore)?.userId.id)
            if (currentPlayerAppliedEffect != undefined) {
                effectFrozen = currentPlayerAppliedEffect.effectTypes.includes(EffectType.Freeze)
                reflectApplied = currentPlayerAppliedEffect.effectTypes.includes(EffectType.Reflect)
            }
            eliminatedPlayers = state.eliminatedPlayers
            isEliminated = eliminatedPlayers.filter(p => p.id == get(globalSessionStore)?.userId.id).length == 1
        }
    })

    $: gridStateStore.subscribe(async state => {
        if (state.currentRowSubmitted && !processingGirdState) {
            console.log("Row submitted")
            processingGirdState = true
            let currentWord = state.getCurrentWord()
            console.log("Current guess", currentWord)
            let result = await TempoGameMakeAGuess({id: gameId}, {word: currentWord.value})
            console.log("MakeAGuess result", result)
            playerStateStore?.setValues(result.playerState)
            await handleGameEvent(result.gameEvent)
            gridStateStore.unlockGrid()
            processingGirdState = false

        }
    })

    function showNotif(time: number) {
        showNotificationStore.set(true)
        setTimeout(() => {
            showNotificationStore.set(false)
        }, time);
    }

    async function handleGameEvent(event: TempoGameEvent) {
        switch (event.eventType) {
            case TempoGameEventType.GuessSuccess:
                console.log("GuessSuccess")
                showNotif(timeBetweenWordSwitch);
                notificationMessageStore.set(event.message)

                let state = await GoToNextWord<TempoGamePlayerState>({id: gameId}, GameType.Tempo);
                updateFromPlayerState(state)

                break
            case TempoGameEventType.WordFailed:
                console.log("WordFailed")
                showNotif(timeBetweenWordSwitch);
                notificationMessageStore.set(event.message)

                console.log("Failed word, going to the next")
                let state1 = await GoToNextWord<TempoGamePlayerState>({id: gameId}, GameType.Tempo);
                await sleep(timeBetweenWordSwitch);
                updateFromPlayerState(state1)
                break
            case TempoGameEventType.GuessIncorrect:
                console.log("GuessIncorrect")
                break
            case TempoGameEventType.GameOver:
                routeToGameResults(GameType.Tempo, {id: gameId})
                break
            default:
                throw new Error("Event undefined!")
        }
    }

    onMount(async () => {
        let status: GameStatus = await GetGameStatus({id: gameId}, GameType.Tempo)
        if (status === GameStatus.NotInitialized) {
            routeToMainMenu()
        } else if (status === GameStatus.Finished) {
            routeToGameResults(GameType.Tempo, {id: gameId})
        } else {
            await signalizeReadiness({id: gameId}, GameType.Tempo)
            await initializeCallbacks()
        }

    })

    let chosenEffectType: EffectType | null;

    function startApplyDisruptiveEffect(type: EffectType) {
        applyingEffect = true
        chosenEffectType = type
    }

    async function applyNonDisruptiveEffect(type: EffectType) {
        let userId = get(globalSessionStore)?.userId
        if (userId != null) {
            let res = await ApplyEffect<TempoGamePlayerState>({id: gameId}, GameType.Tempo, userId, type);
            console.log("applyNonDisruptiveEffect")
            updateFromPlayerState(res)
        }

    }

    async function applyDisruptiveEffect(player: UserId) {
        applyingEffect = false
        if (chosenEffectType != null) {
            let res = await ApplyEffect<TempoGamePlayerState>({id: gameId}, GameType.Tempo, player, chosenEffectType);
            console.log("applyDisruptiveEffect")
            updateFromPlayerState(res)
        }
    }

    //&& playerScore.userId.id !== get(globalSessionStore)?.userId.id
</script>

<style>
    @keyframes pulsate {
        0% {
            transform: scale(1);
            opacity: 1;
        }
        50% {
            transform: scale(1.05);
            opacity: 0.7;
        }
        100% {
            transform: scale(1);
            opacity: 1;
        }
    }

    .pulsate-effect {
        animation: pulsate 2s infinite ease-in-out;
    }

    .vertical-flip {
        transform: rotate(90deg);
        /* Optional: Adjust the position as needed */
        transform-origin: center center; /* Adjust the pivot point of the rotation */
        display: inline-block; /* or block, depending on your layout needs */
    }
</style>

<div class="flex flex-col justify-between h-full">
    {#if hasBegun}
        <div>
            {#if $showNotificationStore}
                <NotificationComponent messageStore="{notificationMessageStore}"/>
            {/if}
            <div class="card rounded-none variant-filled flex justify-between items-center">
                <div class="h-full flex flex-col justify-center">
                    <div class="ml-2 flex flex-col">
                        <div><b>Next Elimination:</b> {Math.ceil(timeUntilNextEliminationMillis / 1000)}s</div>
                        <div><b>Threshold:</b> {eliminationThreshold} points</div>
                    </div>
                </div>
                <div>
                    <button class="btn btn-md" on:click={() => {modalStore.trigger(modal)}}>
                        Leave game
                    </button>
                </div>
            </div>
            <div class="flex flex-wrap text-center justify-center">
                {#each playerScores as playerScore, index}
                    <button on:click={applyingEffect ? () => applyDisruptiveEffect(playerScore.userId) : null}
                            class={"card w-1/2 p-1 rounded-none " + (applyingEffect  ? 'pulsate-effect bg-initial' : 'variant-ghost')}>
                        <div class="flex">
                            <p>{cutNameLenght(playerScore.playerName, 16)}</p>
                            {#if eliminatedPlayers.filter(val => val.id === playerScore.userId.id).length > 0}
                                <span class="badge-icon variant-filled m-1"> 
                                    <MdiDeath/>
                                </span>
                            {/if}
                            {#each allPlayersAppliedEffects.find(e => e.userId.id === playerScore.userId.id)?.effectTypes || [] as appliedEffect, i}
                                {#if appliedEffect === EffectType.Freeze}
                                    <span class="badge-icon variant-filled m-1"> 
                                        <StreamlineInterfaceWeatherSnowFlakeWinterFreezeSnowFreezingIceColdWeatherSnowflake/>
                                    </span>

                                {:else if appliedEffect === EffectType.Reflect && get(globalSessionStore)?.userId.id === playerScore.userId.id }
                                    <span class="badge-icon variant-filled m-1"> 
                                        <GameIconsShieldReflect/>
                                    </span>

                                {:else if appliedEffect === EffectType.KeyboardHints}
                                    <span class="badge-icon variant-filled m-1">
                                        <MaterialSymbolsKeyboardSharp/>
                                    </span>
                                {/if}
                            {/each}
                        </div>
                        <div class="flex">
                            <h4 class="h4">Score: {playerScore.score}</h4>
                        </div>
                    </button>
                {/each}
            </div>
            {#if hint != null}
                <div class="card m-1 variant-ghost flex justify-center p-1 rounded-none">

                    <div class="m-1 p-1 text-center transition-transform"
                         in:slide={{ duration: 500 }}
                         out:slide={{ duration: 500 }}>
                        <p class="text-md font-bold">{HintType[hint.hintType]}:&nbsp;</p>
                        <p class="text-sm">{hint.hintText}</p>
                    </div>
                </div>
            {/if}

        </div>
        <div class="flex justify-between">
            <div>
            </div>
            <div>
                <GridComponent {gridStateStore}></GridComponent>
                <div class="mt-2">
                    <ProgressBar label="Progress Bar" meter="bg-error-500/30" value={tempoGauge} max={100}/>
                    <h4 class="h4">{tempoGauge}</h4>
                </div>
            </div>
            <div class="vertical-flip">

            </div>

        </div>

        <div>

            <div class="card flex justify-evenly m-2 p-1.5">
                <button disabled="{!enoughTempoForAbility || isEliminated}"
                        class="btn-icon  {!enoughTempoForAbility || isEliminated ? 'variant-ghost' : 'variant-filled-success' }"
                        on:click={()=>{startApplyDisruptiveEffect(EffectType.Freeze)}}>
                    <StreamlineInterfaceWeatherSnowFlakeWinterFreezeSnowFreezingIceColdWeatherSnowflake/>
                </button>
                <button disabled="{reflectApplied || !enoughTempoForAbility || isEliminated}"
                        class="btn-icon  {reflectApplied || !enoughTempoForAbility || isEliminated  ? 'variant-ghost' : 'variant-filled-success' }"
                        on:click={()=>{applyNonDisruptiveEffect(EffectType.Reflect)}
                }>
                    <GameIconsShieldReflect></GameIconsShieldReflect>
                </button>
                <button disabled="{!enoughTempoForAbility || isEliminated}"
                        class="btn-icon {!enoughTempoForAbility || isEliminated ? 'variant-ghost' : 'variant-filled-success' }"
                        on:click={()=>{applyNonDisruptiveEffect(EffectType.KeyboardHints)}}
                >
                    <MaterialSymbolsKeyboardSharp/>
                </button>
            </div>
            <Keyboard {gridStateStore} disabled="{effectFrozen || isEliminated}"
                      keyboardHints="{keyboardHints}"></Keyboard>
        </div>

    {:else if hasInitialized }
        <div>
            <TempoGamePregameScreen {sharedStateStore}></TempoGamePregameScreen>
        </div>
    {:else}
        <PreGameLoadScreen></PreGameLoadScreen>
    {/if}
</div>