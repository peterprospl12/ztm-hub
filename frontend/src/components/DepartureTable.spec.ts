import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import DepartureTable from './DepartureTable.vue';
import { ZtmPlugin } from '../plugins/ztmPlugin';
import type { Departure } from '../composables/useZtmData';

describe('DepartureTable', () => {
    const realtimeDeparture: Departure = {
        line: '6',
        direction: 'Jelitkowo',
        scheduledTime: '22:38',
        estimatedTime: '2025-12-07T22:35:54+01:00',
        delaySeconds: -125,
        status: 'REALTIME',
    };

    const scheduledDeparture: Departure = {
        line: '9',
        direction: 'Strzyża PKM',
        scheduledTime: '22:42',
        estimatedTime: '2025-12-07T22:40:49+01:00',
        delaySeconds: 0,
        status: 'SCHEDULED',
    };

    const mockDepartures: Departure[] = [realtimeDeparture, scheduledDeparture];

    it('should display loading state', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: [],
                isLoading: true,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        expect(wrapper.text()).toContain('Loading departures...');
    });

    it('should display empty state when no departures', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: [],
                isLoading: false,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        expect(wrapper.text()).toContain('No departures found');
    });

    it('should display departures correctly', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: mockDepartures,
                isLoading: false,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        expect(wrapper.text()).toContain('6');
        expect(wrapper.text()).toContain('Jelitkowo');
        expect(wrapper.text()).toContain('9');
        expect(wrapper.text()).toContain('Strzyża PKM');
    });

    it('should render correct number of rows', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: mockDepartures,
                isLoading: false,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        const rows = wrapper.findAll('tbody tr');
        expect(rows).toHaveLength(2);
    });

    it('should show Live status for REALTIME', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: [realtimeDeparture],
                isLoading: false,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        expect(wrapper.text()).toContain('Live');
    });

    it('should show Scheduled status for SCHEDULED', () => {
        const wrapper = mount(DepartureTable, {
            props: {
                departures: [scheduledDeparture],
                isLoading: false,
            },
            global: {
                plugins: [ZtmPlugin],
            },
        });

        expect(wrapper.text()).toContain('Scheduled');
    });
});