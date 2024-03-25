import {elasticOut} from "svelte/easing";

export function sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export function cutNameLenght(name: string, lenght: number) {
    return name.slice(0, 16) + (name.length > 16 ? '...' : '')
}

export type SpinParams = {
    duration: number;
};

// Define a type for the return value
export type TransitionReturnType = {
    duration: number;
    css: (t: number) => string;
};

export function spin(node: HTMLElement, {duration}: SpinParams): TransitionReturnType {
    return {
        duration,
        css: (t: number) => {
            const eased = elasticOut(t);

            return `
                transform: scale(${eased}) rotate(${eased * 1080}deg);
                color: hsl(
                    ${~~(t * 360)},
                    ${Math.min(100, 1000 - 1000 * t)}%,
                    ${Math.min(50, 500 - 500 * t)}%
                );`;
        }
    };
}