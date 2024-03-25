<script>
    import LoginForm from "$lib/components/auth/LoginForm.svelte";
    import GameTitle from "$lib/components/core/GameTitle.svelte";
    import {userStore} from "$lib/firebase";
    import {routeToLogin, routeToMainMenu} from "$lib/routing";
    import {browser} from "$app/environment";

    $: if (browser) {
        userStore.subscribe(async user => {
            if (user) {
                routeToMainMenu()
            } else {
                routeToLogin()
            }
        })
    }

</script>


<GameTitle></GameTitle>
<LoginForm></LoginForm>


