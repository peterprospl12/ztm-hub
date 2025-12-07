import { ref } from 'vue';
import apiClient from '../api/axios';

export interface Departure {
    line: string;
    direction: string;
    estimatedTime: string;
    scheduledTime: string;
    delaySeconds: number;
    status: string;
}

export interface UserStop {
    id: string;
    stopId: number;
    displayName: string | null;
    addedAt: string;
}

export interface DeparturesResponse {
    stopId: number;
    stopName: string;
    departures: Departure[];
}

export function useZtmData() {
    const userStops = ref<UserStop[]>([]);
    const departures = ref<Departure[]>([]);
    const currentStopName = ref<string | null>(null);
    const isLoading = ref(false);
    const error = ref<string | null>(null);

    async function fetchUserStops() {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await apiClient.get('/Stops');
            userStops.value = response.data;
        } catch (e: any) {
            error.value = e.response?.data?.message || 'Failed to fetch stops';
            console.error('Failed to fetch user stops:', e);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchDepartures(stopId: number): Promise<DeparturesResponse> {
        isLoading.value = true;
        error.value = null;
        try {
            const response = await apiClient.get(`/Stops/${stopId}/departures`);
            const data: DeparturesResponse = response.data;

            departures.value = data.departures || [];
            currentStopName.value = data.stopName || null;

            return data;
        } catch (e: any) {
            error.value = e.response?.data?.message || 'Failed to fetch departures';
            console.error('Failed to fetch departures:', e);
            return { stopId, stopName: '', departures: [] };
        } finally {
            isLoading.value = false;
        }
    }

    async function addStop(stopId: number, customName?: string) {
        isLoading.value = true;
        error.value = null;
        try {
            await apiClient.post('/Stops', { stopId, customName });
            await fetchUserStops();
            return true;
        } catch (e: any) {
            error.value = e.response?.data?.message || 'Failed to add stop';
            console.error('Failed to add stop:', e);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function removeStop(userStopId: string) {
        isLoading.value = true;
        error.value = null;
        try {
            await apiClient.delete(`/Stops/${userStopId}`);
            await fetchUserStops();
            return true;
        } catch (e: any) {
            error.value = e.response?.data?.message || 'Failed to remove stop';
            console.error('Failed to remove stop:', e);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function updateStop(userStopId: string, displayName: string) {
        isLoading.value = true;
        error.value = null;
        try {
            await apiClient.put(`/Stops/${userStopId}`, { displayName });
            await fetchUserStops();
            return true;
        } catch (e: any) {
            error.value = e.response?.data?.message || 'Failed to update stop';
            console.error('Failed to update stop:', e);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    return {
        userStops,
        departures,
        currentStopName,
        isLoading,
        error,
        fetchUserStops,
        fetchDepartures,
        addStop,
        removeStop,
        updateStop,
    };
}