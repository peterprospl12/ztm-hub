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
        el.classList.add('text-green-500');
    } else if (delay <= 120) {
        el.classList.add('text-yellow-500');
    } else {
        el.classList.add('text-red-500');
    }
}

export default vDelayColor;