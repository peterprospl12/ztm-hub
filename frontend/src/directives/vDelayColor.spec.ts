import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import { vDelayColor } from './vDelayColor';

describe('vDelayColor directive', () => {
    const TestComponent = {
        template: '<span v-delay-color="delay">Test</span>',
        props: ['delay'],
        directives: { 'delay-color': vDelayColor },
    };

    it('should add green class for 0 delay', () => {
        const wrapper = mount(TestComponent, { props: { delay: 0 } });
        expect(wrapper.find('span').classes()).toContain('text-green-500');
    });

    it('should add green class for negative delay (early)', () => {
        const wrapper = mount(TestComponent, { props: { delay: -60 } });
        expect(wrapper.find('span').classes()).toContain('text-green-500');
    });

    it('should add yellow class for small delay (1-120s)', () => {
        const wrapper = mount(TestComponent, { props: { delay: 60 } });
        expect(wrapper.find('span').classes()).toContain('text-yellow-500');
    });

    it('should add red class for large delay (>120s)', () => {
        const wrapper = mount(TestComponent, { props: { delay: 180 } });
        expect(wrapper.find('span').classes()).toContain('text-red-500');
    });

    it('should add green class for null delay', () => {
        const wrapper = mount(TestComponent, { props: { delay: null } });
        expect(wrapper.find('span').classes()).toContain('text-green-500');
    });

    it('should update class when delay changes', async () => {
        const wrapper = mount(TestComponent, { props: { delay: 0 } });
        expect(wrapper.find('span').classes()).toContain('text-green-500');

        await wrapper.setProps({ delay: 180 });
        expect(wrapper.find('span').classes()).toContain('text-red-500');
        expect(wrapper.find('span').classes()).not.toContain('text-green-500');
    });
});