<script lang="ts">
    import {routeToMatchHistory, routeToMatchmaking, routeToPlay} from "$lib/routing";
    import {GameType} from "$lib/generate/domain/game-type";
    import type {ModalSettings} from "@skeletonlabs/skeleton";
    import {getModalStore} from '@skeletonlabs/skeleton';

    const modalStore = getModalStore();
    const modal: ModalSettings = {
        type: 'alert',
        title: 'How to play?',
        body: '' +
            '<li>Try to guess as many words as possible</li>' +
            '<li><b class="variant-filled-success">Green</b> letters are in the <b>correct</b> spot</li>' +
            '<li><b class="variant-filled-warning">Yellow</b> letters are <b>not</b> in the <b>correct</b> spot</li>' +
            '<li>If your score is not <b>high</b> enough, you will be <b>eliminated</b></li>' +
            '<li>Use the <b>powers</b> you unlock to be the last player standing</li>'
    };

</script>

<div class="card w-5/6 max-w-xl">
    <header class="card-header flex flex-col justify-center items-center text-center">
        <h3 class="h3">Welcome to the ultimate word showdown.</h3>
        <h5 class="h5">May the best wordsmith win!</h5>
    </header>
    <section class="p-4 flex flex-col justify-center items-center">

        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={routeToPlay}
                type="button">
            <span>Play!</span>
        </button>
        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={() => routeToMatchmaking(GameType.Practice)}
                type="button">
            <span>Practice</span>
        </button>
        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={routeToMatchHistory}
                type="button">
            <span>Match history</span>
        </button>
        <button class="btn variant-filled mb-3 w-3/4 transition ease-in-out hover:-translate-y-1"
                on:click={()=> {modalStore.trigger(modal)}}
                type="button">
            <span>How to play?</span>
        </button>
    </section>
</div>