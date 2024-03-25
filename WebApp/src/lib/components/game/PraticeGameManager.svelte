<script lang="ts">
    import {createGirdStateStore} from "$lib/stores/gridState.ts";
    import Grid from "$lib/components/game/GridComponent.svelte";
    import {onMount} from "svelte";
    import {slide} from 'svelte/transition';
    import {getGameClient} from "$lib/clients/gameclient.ts";
    import type {PracticeGameUserStateStore} from "$lib/stores/practiceGameState.ts";
    import {createPracticeGameUserStateStore} from "$lib/stores/practiceGameState.ts";
    import {PracticeGameEventType} from "$lib/generate/domain/practice-game-event-type.ts";
    import {sleep} from "$lib/util.ts";
    import type {PracticeGamePlayerState} from "$lib/generate/domain/practice-game-player-state.ts";
    import type {GuessResult} from "$lib/generate/domain/guess-result.ts";
    import Keyboard from "$lib/components/game/Keyboard.svelte";
    import PracticeGameScoreboard from "$lib/components/game/PracticeGameScoreboard.svelte";
    import {GameType} from "$lib/generate/domain/game-type.ts";
    import type {PracticeGameEvent} from "$lib/generate/domain/practice-game-event.ts";
    import {
        GetPlayerState,
        GoToNextWord,
        PracticeGameMakeAGuess,
        signalizeReadiness
    } from "$lib/clients/common-game-client.ts";
    import NotificationComponent from "$lib/components/game/NotificationComponent.svelte";
    import {writable} from "svelte/store";
    import {routeToMainMenu} from "$lib/routing.ts";
    import type {ModalSettings} from "@skeletonlabs/skeleton";
    import {getModalStore} from "@skeletonlabs/skeleton";

    const modalStore = getModalStore();
    const modal: ModalSettings = {
        type: 'confirm',
        title: '',
        body: 'Are you sure you want to return to the menu?',
        response: (r: boolean) => {
            if (r) routeToMainMenu()
        }
    };
    export let gameId: string
    let gridStateStore = createGirdStateStore(4, 5)
    let practiceGamePlayerStateStore: PracticeGameUserStateStore | null = null
    let gameReady = false;
    let processingGirdState = false
    let guessedWordCount = 0
    let failedWordCount = 0

    let notificationMessageStore = writable("Initial")
    let timeBetweenWordSwitch = 1500
    let gridRecreateTimestamp = 0


    async function initializePracticeGame() {
        let client = await getGameClient()


        if (client != null) {
            let res = await GetPlayerState<PracticeGamePlayerState>({id: gameId}, GameType.Practice)
            console.log("PracticeGamePlayerState", res)
            initializeFromPlayerState(res);
            gameReady = true
        }
    }

    function initializeFromPlayerState(res: PracticeGamePlayerState) {
        practiceGamePlayerStateStore = createPracticeGameUserStateStore(res)
        gridStateStore = createGirdStateStore(res.currentWordLenght, res.allowedGuesses)
        gridRecreateTimestamp = Date.now()
    }

    const loopTaskPeriod = 2000

    function startLoopTasks() {
        setTimeout(async () => {
            await signalizeReadiness({id: gameId}, GameType.Practice)
        }, loopTaskPeriod)
    }

    onMount(
        async () => {
            startLoopTasks()
            await initializePracticeGame();
        }
    )

    // Handle word guess
    $: gridStateStore.subscribe(async state => {
        if (state.currentRowSubmitted && !processingGirdState) {
            console.log("Row submitted")
            processingGirdState = true
            let currentWord = state.getCurrentWord()

            let result = await PracticeGameMakeAGuess({id: gameId}, {word: currentWord.value})
            console.log("MakeAGuess result", result)

            practiceGamePlayerStateStore?.setValues(result.playerState)
            await handleGameEvent(result.gameEvent)
            gridStateStore.unlockGrid()
            processingGirdState = false
        }
    })
    let showNotificationStore = writable(false);

    function showNotif(time: number) {
        showNotificationStore.set(true)
        setTimeout(() => {
            showNotificationStore.set(false)
        }, time);
    }

    async function handleGameEvent(event: PracticeGameEvent) {
        switch (event.eventType) {
            case PracticeGameEventType.GuessSuccess:
                notificationMessageStore.set(event.message)
                showNotif(timeBetweenWordSwitch)
                let state = await GoToNextWord({id: gameId}, GameType.Practice);
                initializeFromPlayerState((state as PracticeGamePlayerState))
                break
            case PracticeGameEventType.WordFailed:
                notificationMessageStore.set(event.message)
                showNotif(timeBetweenWordSwitch)

                let state1 = await GoToNextWord({id: gameId}, GameType.Practice);
                await sleep(timeBetweenWordSwitch);
                initializeFromPlayerState((state1 as PracticeGamePlayerState))
                break
            case PracticeGameEventType.GuessFailed:
                break

            default:
                throw new Error("Event undefined!")
        }
    }

    // Reacting to updates in state

    $: practiceGamePlayerStateStore?.subscribe(state => {
        let row = 0
        let map = new Map<number, GuessResult>()
        guessedWordCount = state.guessedWordCount
        failedWordCount = state.failedWordCount
        state.currentWordGuessResults.forEach(result => {
            map.set(row, result)
            row++;
        })
        gridStateStore.updateLetterState(map)

    })

</script>
{#if $showNotificationStore}
    <NotificationComponent messageStore="{notificationMessageStore}"/>
{/if}
<div class="flex flex-col justify-between h-full">
    <div>
        <div class="card rounded-none variant-filled flex justify-between items-center">
            <div class="h-full flex flex-col justify-center">
            </div>
            <div>
                <button class="btn btn-md" on:click={() => {modalStore.trigger(modal)}}>
                    Return to menu
                </button>
            </div>
        </div>
        <PracticeGameScoreboard {failedWordCount} {guessedWordCount}></PracticeGameScoreboard>
    </div>

    {#key gridRecreateTimestamp}
        <div transition:slide={{ duration: 300 }}>
            <Grid {gridStateStore}></Grid>
        </div>
    {/key}

    <div>
        <Keyboard disabled="{false}" {gridStateStore}></Keyboard>
    </div>
</div>
