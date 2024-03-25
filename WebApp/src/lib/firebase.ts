
import { initializeApp } from "firebase/app";
import { getAuth, onAuthStateChanged } from "firebase/auth";
import { writable } from "svelte/store";
import type { User } from "firebase/auth";

const firebaseConfig = {
    apiKey: "AIzaSyBVzz_FJDLqpPsurE_EsHvvlAkcgrKiBZU",
    authDomain: "worlde-arena.firebaseapp.com",
    projectId: "worlde-arena",
    storageBucket: "worlde-arena.appspot.com",
    messagingSenderId: "686603065231",
    appId: "1:686603065231:web:b619d26c8028e37c67ddfa",
    measurementId: "G-ET3XFRQEEQ"
  };


  // Initialize Firebase
export const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);

function createUserStore() {
  let unsubscribe: () => void;

  if (!auth || !globalThis.window) {
    console.warn('Auth is not initialized or not in browser');
    const { subscribe } = writable<User | null>(null);
    return {
      subscribe,
    }
  }

  const { subscribe } = writable(auth?.currentUser ?? null, (set) => {
    unsubscribe = onAuthStateChanged(auth, (user: any) => {
      set(user);
    });

    return () => unsubscribe();
  });

  return {
    subscribe,
  };
}

export const userStore = createUserStore();