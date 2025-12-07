import type { Directive } from 'vue';

export const vDelayColor: Directive<HTMLElement, number | null | undefined> = {
    mounted(el, binding) {
        applyColor(el, binding.value);
    },
    updated(el, binding) {
        applyColor(el, binding.value);
    },
};

function applyColor(el: HTMLElement, delay: number | null | undefined) {
    el.classList.remove('text-green-500', 'text-yellow-500', 'text-red-500');

    if (delay == null || delay <= 0) {
        // Na czas lub wcześniej
        el.classList.add('text-green-500');
    } else if (delay <= 120) {
        // Lekkie opóźnienie (do 2 min)
        el.classList.add('text-yellow-500');
    } else {
        // Znaczące opóźnienie
        el.classList.add('text-red-500');
    }
}

export default vDelayColor;