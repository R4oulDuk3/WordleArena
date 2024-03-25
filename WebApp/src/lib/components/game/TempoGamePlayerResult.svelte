<script lang="ts">
    import {onMount} from "svelte";
    import type {GameId} from "$lib/generate/domain/game-id.ts";
    import {GetTempoGameResult} from "$lib/clients/common-game-client.ts";
    import type {GetTempoGameResultResponse} from "$lib/generate/messages/get-tempo-game-result-response.ts";
    import {GetTempoGameResultResponseCode} from "$lib/generate/messages/get-tempo-game-result-response-code.ts";
    import Emojione1stPlaceMedal from '~icons/emojione/1st-place-medal';
    import Emojione2ndPlaceMedal from '~icons/emojione/2nd-place-medal';
    import Emojione3rdPlaceMedal from '~icons/emojione/3rd-place-medal';
    import PhSkullBold from '~icons/ph/skull-bold';
    import {cutNameLenght} from "$lib/util.ts";
    import {routeToMainMenu} from "$lib/routing.ts";
    import type {ConicStop} from "@skeletonlabs/skeleton";
    import {ConicGradient} from "@skeletonlabs/skeleton";
    import {get} from "svelte/store";
    import {globalSessionStore} from "$lib/stores/user-session.ts";

    export let gameId: GameId
    let code = GetTempoGameResultResponseCode.NotFound
    let gameResult: GetTempoGameResultResponse | null = null
    let statisticsAvailable = false
    let wordsGuessed = 0
    let wordsFailed = 0
    let conicStops: ConicStop[] = [
        {color: 'rgba(255,255,255,1)', start: 0, end: 10, label: "abc"},
        {color: 'rgba(255,255,255,0.5)', start: 10, end: 35, label: "abc"},
        {color: 'rgba(255,255,255,0.25)', start: 35, end: 100, label: "abc"}
    ];
    onMount(async () => {

        gameResult = await GetTempoGameResult(gameId)
        code = gameResult.code
        if (code != GetTempoGameResultResponseCode.Ok) return
        gameResult.tempoGamePlayerResultDtos = gameResult.tempoGamePlayerResultDtos.sort((p1, p2) =>
            p1.resultInfo.place > p2.resultInfo.place ? 1 : -1
        )
        let dto = gameResult.tempoGamePlayerResultDtos.find(dto => dto.userId.id == get(globalSessionStore)?.userId.id)
        statisticsAvailable = dto != undefined;
        if (dto != undefined) {
            let guessDistributions = dto.resultInfo.guessDistributions
            conicStops = []
            wordsGuessed = guessDistributions.reduce((a, b) => {
                return a + b.wordCount
            }, 0);
            wordsFailed = dto.resultInfo.failedWords
            let inc = 0;
            var cnt = 0
            guessDistributions.forEach(d => {
                let percent = (d.wordCount * 1.0 / wordsGuessed) * 100
                cnt++;
                conicStops.push(
                    {
                        color: `rgba(255,123,123, ${cnt / guessDistributions.length})`,
                        start: inc,
                        end: inc + percent,
                        label: "Try " + d.guessedOnTry
                    },
                )
                inc += percent
            })
        }


    })

</script>
{#if code === GetTempoGameResultResponseCode.NotFound || gameResult == null}
    not found
{:else if code === GetTempoGameResultResponseCode.NotAllowed}
    not allowed
{:else}
    <div class="card variant-ghost m-4 flex flex-col items-center w-4/5 sm:w-2/3 md:w-1/2 rounded-none">
        <div class="card p-3 w-full flex justify-center rounded-none variant-filled">
            <h1 class="h1">Game Results</h1>
        </div>
        <div class="w-full m-2 flex flex-wrap">
            <div class=" pl-4 pr-4 flex flex-col justify-center w-full">
                <h3 class="h3 mb-1.5">Scores</h3>
                <ol class="list w-full">
                    {#each gameResult.tempoGamePlayerResultDtos as dto}
                        <li class="card p-3 flex justify-between">
                            <div class="flex items-center">
                            <span>
                                <h2 class="h2">
                                    {#if dto.resultInfo.place === 1}
                                        <svelte:component this={Emojione1stPlaceMedal}/>
                                    {:else if dto.resultInfo.place === 2}
                                        <svelte:component this={Emojione2ndPlaceMedal}/>
                                    {:else if dto.resultInfo.place === 3}
                                        <svelte:component this={Emojione3rdPlaceMedal}/>
                                    {:else}
                                        <svelte:component this={PhSkullBold}/>
                                    {/if}
                                </h2>
                            </span>
                                <span class="ml-1">{cutNameLenght(dto.playerInfo.playerName, 16)}</span>
                            </div>
                            <span class="pr-2">{dto.resultInfo.score}</span>
                        </li>
                    {/each}
                </ol>
            </div>
            <div class=" pl-4 pr-4 flex flex-col justify-center w-full">
                <h3 class="h3 mb-1.5">Statistics</h3>
                <div class="flex justify-around">
                    <div class="flex flex-col justify-center items-center">
                        <p>Guessed Words</p>
                        <h3 class="h3">{wordsGuessed}</h3>
                    </div>
                    <div class="flex flex-col justify-center items-center">
                        <p>
                            Failed Words
                        </p>
                        <h3 class="h3">{wordsFailed}</h3>
                    </div>
                </div>
                <ConicGradient stops={conicStops} legend={true} regionLegend={"grid gap-2 grid-cols-2"}>
                    <h5 class="h5">Words guessed by number of tries</h5>
                </ConicGradient>
            </div>
        </div>
        <div class="flex justify-center mb-4">
            <button type="button" class="btn btn-md variant-filled" on:click={routeToMainMenu}>Return</button>
        </div>
    </div>
{/if}
