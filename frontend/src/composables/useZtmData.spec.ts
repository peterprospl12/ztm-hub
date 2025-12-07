import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useZtmData } from './useZtmData';
import { setActivePinia, createPinia } from 'pinia';

// Mock axios
vi.mock('../api/axios', () => ({
    default: {
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
    },
}));

import apiClient from '../api/axios';

describe('useZtmData', () => {
    beforeEach(() => {
        setActivePinia(createPinia());
        vi.clearAllMocks();
    });

    describe('fetchUserStops', () => {
        it('should fetch user stops successfully', async () => {
            const mockStops = [
                { id: '1', stopId: 2019, displayName: 'Home', addedAt: '2025-12-07' },
                { id: '2', stopId: 2020, displayName: null, addedAt: '2025-12-07' },
            ];

            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: mockStops });

            const { userStops, fetchUserStops, isLoading, error } = useZtmData();

            expect(isLoading.value).toBe(false);

            await fetchUserStops();

            expect(apiClient.get).toHaveBeenCalledWith('/Stops');
            expect(userStops.value).toEqual(mockStops);
            expect(error.value).toBeNull();
        });

        it('should handle fetch error', async () => {
            vi.mocked(apiClient.get).mockRejectedValueOnce({
                response: { data: { message: 'Network error' } }
            });

            const { userStops, fetchUserStops, error } = useZtmData();

            await fetchUserStops();

            expect(userStops.value).toEqual([]);
            expect(error.value).toBe('Network error');
        });
    });

    describe('fetchDepartures', () => {
        it('should fetch departures successfully', async () => {
            const mockResponse = {
                stopId: 2019,
                stopName: 'Miszewskiego',
                departures: [
                    { line: '6', direction: 'Jelitkowo', delaySeconds: 0, scheduledTime: '22:38', estimatedTime: '22:38', status: 'REALTIME' },
                ],
            };

            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: mockResponse });

            const { fetchDepartures, currentStopName, departures } = useZtmData();

            const result = await fetchDepartures(2019);

            expect(apiClient.get).toHaveBeenCalledWith('/Stops/2019/departures');
            expect(result.stopName).toBe('Miszewskiego');
            expect(result.departures).toHaveLength(1);
            expect(currentStopName.value).toBe('Miszewskiego');
            expect(departures.value).toHaveLength(1);
        });

        it('should handle empty departures', async () => {
            const mockResponse = {
                stopId: 2019,
                stopName: 'Test Stop',
                departures: [],
            };

            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: mockResponse });

            const { fetchDepartures, departures } = useZtmData();

            const result = await fetchDepartures(2019);

            expect(result.departures).toHaveLength(0);
            expect(departures.value).toHaveLength(0);
        });
    });

    describe('addStop', () => {
        it('should add stop successfully', async () => {
            vi.mocked(apiClient.post).mockResolvedValueOnce({ data: {} });
            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: [] });

            const { addStop } = useZtmData();

            const result = await addStop(2019, 'My Stop');

            expect(apiClient.post).toHaveBeenCalledWith('/Stops', { stopId: 2019, customName: 'My Stop' });
            expect(result).toBe(true);
        });

        it('should add stop without custom name', async () => {
            vi.mocked(apiClient.post).mockResolvedValueOnce({ data: {} });
            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: [] });

            const { addStop } = useZtmData();

            const result = await addStop(2019);

            expect(apiClient.post).toHaveBeenCalledWith('/Stops', { stopId: 2019, customName: undefined });
            expect(result).toBe(true);
        });
    });

    describe('removeStop', () => {
        it('should remove stop successfully', async () => {
            vi.mocked(apiClient.delete).mockResolvedValueOnce({ data: {} });
            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: [] });

            const { removeStop } = useZtmData();

            const result = await removeStop('uuid-123');

            expect(apiClient.delete).toHaveBeenCalledWith('/Stops/uuid-123');
            expect(result).toBe(true);
        });

        it('should handle remove error', async () => {
            vi.mocked(apiClient.delete).mockRejectedValueOnce({
                response: { data: { message: 'Not found' } }
            });

            const { removeStop, error } = useZtmData();

            const result = await removeStop('uuid-123');

            expect(result).toBe(false);
            expect(error.value).toBe('Not found');
        });
    });

    describe('updateStop', () => {
        it('should update stop successfully', async () => {
            vi.mocked(apiClient.put).mockResolvedValueOnce({ data: {} });
            vi.mocked(apiClient.get).mockResolvedValueOnce({ data: [] });

            const { updateStop } = useZtmData();

            const result = await updateStop('uuid-123', 'New Name');

            expect(apiClient.put).toHaveBeenCalledWith('/Stops/uuid-123', { displayName: 'New Name' });
            expect(result).toBe(true);
        });
    });
});