<script>

    import {auth} from "$lib/firebase";
    import {GoogleAuthProvider, signInAnonymously, signInWithPopup} from "firebase/auth";

    async function handleGoogleSignIn() {
        const provider = new GoogleAuthProvider();
        try {
            const result = await signInWithPopup(auth, provider);
            console.log(result);
            // User signed in
            // Redirect or update UI
        } catch (error) {
            console.error(error);
        }
    }

    // // Anonymous sign-in
    async function handleAnonymousSignIn() {
        try {
            const result = await signInAnonymously(auth);
            console.log(result);
            // User signed in anonymously
            // Redirect or update UI
        } catch (error) {
            console.error(error);
        }
    }
</script>

<div class="card">
    <header class="card-header flex flex-col justify-center items-center">
        <h3 class="h3">Wanna join?</h3>
        <p>Hope you are ready for a challenge</p>
    </header>
    <section class="p-4 flex flex-col justify-center items-center">
        <form action="flex-col items-center justify-center w-full">
            <label class="label">
                <span>Email</span>
                <input class="input w-full my-2" placeholder="Email" type="text"/>
            </label>

            <label class="label">
                <span>Password</span>
                <input class="input w-full my-2" placeholder="Password" type="text"/>
            </label>

            <div class="flex items-center my-3">
                <div class="flex-grow border-t border-gray-300"></div>
                <span class="flex-shrink mx-4 text-gray-500">OR</span>
                <div class="flex-grow border-t border-gray-300"></div>
            </div>

        </form>

        <button class="btn variant-filled w-full mb-3" on:click={handleAnonymousSignIn} type="button">
            <span>Join as a Guest</span>
        </button>

        <button class="btn variant-filled w-full mb-3" on:click={handleGoogleSignIn} type="button">
            <svg height="1em" viewBox="0 0 488 512" xmlns="http://www.w3.org/2000/svg">
                <!--! Font Awesome Free 6.4.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2023 Fonticons, Inc. -->
                <path d="M488 261.8C488 403.3 391.1 504 248 504 110.8 504 0 393.2 0 256S110.8 8 248 8c66.8 0 123 24.5 166.3 64.9l-67.5 64.9C258.5 52.6 94.3 116.6 94.3 256c0 86.5 69.1 156.6 153.7 156.6 98.2 0 135-70.4 140.8-106.9H248v-85.3h236.1c2.3 12.7 3.9 24.9 3.9 41.4z"/>
            </svg>
            <span>Login with Google</span>
        </button>
    </section>
</div>