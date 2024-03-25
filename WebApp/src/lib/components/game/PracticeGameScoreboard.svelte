<script lang="ts">

    export let guessedWordCount: number
    export let failedWordCount: number
</script>

<div>
    <div class=" p-2 m-1 flex justify-evenly rounded-none">
        <div class="card variant-ghost-success p-4 flex flex-col items-center rounded-md w-2/5">
            <h4 class="h4">{guessedWordCount}</h4>
            <p>Guessed words</p>
        </div>
        <div class="card variant-ghost-error p-4 flex flex-col items-center rounded-md w-2/5">
            <h4 class="h4">{failedWordCount}</h4>
            <p>Failed words</p>
        </div>
    </div>
</div>