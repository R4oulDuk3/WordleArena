<script lang="ts">

    import type {UserId} from "$lib/generate/domain/user-id.ts";
    import {onMount} from "svelte";
    import type {MatchHistoryRecord} from "$lib/generate/domain/match-history-record.ts";
    import {getMatchHistory} from "$lib/clients/match-history-api.ts";
    import {ApiResponseStatusCode} from "$lib/generate/messages/api-response-status-code.ts";
    import {routeToGameResults, routeToMainMenu} from "$lib/routing.ts";
    import type {PaginationSettings} from "@skeletonlabs/skeleton";
    import {Paginator} from "@skeletonlabs/skeleton";
    import {GameType} from "$lib/generate/domain/game-type.ts";

    export let userId: UserId
    let loaded = false
    let responseCode = ApiResponseStatusCode
    let matchHistoryRecords: MatchHistoryRecord[]
    let page = 0
    let total = 0

    let paginationSettings = {
        page: page,
        limit: 10,
        size: total,
        amounts: [],
    } satisfies PaginationSettings;

    async function pullHistory() {
        loaded = false
        let matchHistory = await getMatchHistory(page)
        if (matchHistory.statusCode == ApiResponseStatusCode.Ok && matchHistory.data != null) {
            matchHistoryRecords = matchHistory.data?.records ?? []
            page = matchHistory.data?.page
            total = matchHistory.data?.total
            console.log("matchHistory", matchHistory)
            paginationSettings = {
                page: page,
                limit: 10,
                size: total,
                amounts: [],
            } satisfies PaginationSettings;
        }
        loaded = true
    }

    onMount(async () => {
        await pullHistory();

    })

</script>

{#if loaded}
    <div class="card w-5/6 max-w-xl">
        <header class="card-header flex justify-between items-center text-center">
            <button class="btn variant-ghost" on:click={routeToMainMenu} type="button">{"<"}</button>
            <h3 class="h3">Match History</h3>
            <div></div>
        </header>

        <section class="p-4 flex flex-col justify-center items-center">
            <p class="mb-1">Click on the row to see more info</p>
            <div class="table-container">
                <table class="table table-interactive">
                    <thead>
                    <tr>
                        <th>Finished at</th>
                        <th>Place</th>
                        <th>Score</th>
                    </tr>
                    </thead>
                    <tbody>
                    {#each matchHistoryRecords as record}
                        <tr on:click={()=>{routeToGameResults(GameType.Tempo, record.gameId)}}>
                            <td>{record.finishedAt.toLocaleString().match(/^(\d{4}-\d{2}-\d{2})/)?.[0] || 'No datetime string found'}</td>
                            <td>{record.place}</td>
                            <td>{record.score}</td>
                        </tr>
                    {/each}
                    </tbody>
                </table>
            </div>

            <Paginator
                    bind:settings={paginationSettings}
                    showFirstLastButtons="{false}"
                    showPreviousNextButtons="{true}"
            />

        </section>
    </div>
{/if}