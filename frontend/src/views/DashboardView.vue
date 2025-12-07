<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useZtmData, type Departure, type UserStop } from '../composables/useZtmData';
import StopCard from '../components/StopCard.vue';
import DepartureTable from '../components/DepartureTable.vue';
import StopMap from '../components/StopMap.vue';

const authStore = useAuthStore();
const { userStops, currentStopName, isLoading, error, fetchUserStops, fetchDepartures, addStop, removeStop, updateStop } = useZtmData();

// Form state
const newStopId = ref<number | null>(null);
const newStopName = ref('');
const selectedStop = ref<UserStop | null>(null);
const stopDepartures = ref<Departure[]>([]);
const loadingDepartures = ref(false);

onMounted(() => {
  fetchUserStops();
});

async function handleAddStop() {
  if (newStopId.value) {
    await addStop(newStopId.value, newStopName.value || undefined);
    newStopId.value = null;
    newStopName.value = '';
  }
}

async function handleRemoveStop(userStopId: string) {
  if (confirm('Are you sure you want to remove this stop?')) {
    await removeStop(userStopId);
    if (selectedStop.value?.id === userStopId) {
      selectedStop.value = null;
      stopDepartures.value = [];
    }
  }
}

async function handleSelectStop(stop: UserStop) {
  selectedStop.value = stop;
  loadingDepartures.value = true;
  const response = await fetchDepartures(stop.stopId);
  stopDepartures.value = response.departures;
  loadingDepartures.value = false;
}

// Obsługa kliknięcia na marker na mapie
function handleMapSelectStop(stopId: number) {
  const stop = userStops.value.find(s => s.stopId === stopId);
  if (stop) {
    handleSelectStop(stop);
  }
}

async function handleUpdateStop(userStopId: string, displayName: string) {
  await updateStop(userStopId, displayName);
}
</script>

<template>
  <div class="min-h-screen bg-gray-900 text-white p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <header class="flex justify-between items-center mb-8 border-b border-gray-700 pb-4">
        <h1 class="text-3xl font-bold">ZTM Hub</h1>
        <div class="flex items-center gap-4">
          <span>Welcome, {{ authStore.userEmail }}</span>
          <button @click="authStore.logout" class="bg-red-600 hover:bg-red-700 px-4 py-2 rounded text-sm">
            Logout
          </button>
        </div>
      </header>

      <!-- Error message -->
      <div v-if="error" class="bg-red-600/20 border border-red-500 text-red-300 px-4 py-3 rounded mb-4">
        {{ error }}
      </div>

      <!-- Top row: Add Stop + Your Stops + Map -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
        <!-- Left column: Add stop + List -->
        <div class="lg:col-span-1 space-y-6">
          <!-- Add new stop form -->
          <div class="bg-gray-800 rounded-lg p-4">
            <h2 class="text-xl font-semibold mb-4">Add New Stop</h2>
            <form @submit.prevent="handleAddStop" class="space-y-3">
              <div>
                <label class="block text-sm text-gray-400 mb-1">Stop ID</label>
                <input
                  v-model.number="newStopId"
                  type="number"
                  placeholder="e.g. 2019"
                  class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-blue-500 focus:outline-none"
                  required
                />
              </div>
              <div>
                <label class="block text-sm text-gray-400 mb-1">Custom Name (optional)</label>
                <input
                  v-model="newStopName"
                  type="text"
                  placeholder="e.g. Home"
                  class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-blue-500 focus:outline-none"
                />
              </div>
              <button
                type="submit"
                :disabled="isLoading"
                class="w-full py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-800 disabled:cursor-not-allowed rounded font-semibold transition"
              >
                {{ isLoading ? 'Adding...' : 'Add Stop' }}
              </button>
            </form>
          </div>

          <!-- Stops list -->
          <div class="bg-gray-800 rounded-lg p-4">
            <h2 class="text-xl font-semibold mb-4">Your Stops</h2>

            <div v-if="isLoading && userStops.length === 0" class="text-gray-400 text-center py-4">
              Loading...
            </div>

            <div v-else-if="userStops.length === 0" class="text-gray-400 text-center py-4">
              No stops added yet. Add your first stop above!
            </div>

            <ul v-else class="space-y-2">
              <StopCard
                v-for="stop in userStops"
                :key="stop.id"
                :stop="stop"
                :is-selected="selectedStop?.id === stop.id"
                @select="handleSelectStop"
                @remove="handleRemoveStop"
                @update="handleUpdateStop"
              />
            </ul>
          </div>
        </div>

        <!-- Right column: Map (2/3 width) -->
        <div class="lg:col-span-2">
          <div class="bg-gray-800 rounded-lg p-4 h-full">
            <h2 class="text-xl font-semibold mb-4">Map</h2>
            <div class="h-[450px]">
              <StopMap
                :user-stops="userStops"
                :selected-stop-id="selectedStop?.stopId"
                @select-stop="handleMapSelectStop"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Bottom row: Departures (full width) -->
      <div class="bg-gray-800 rounded-lg p-4">
        <h2 class="text-xl font-semibold mb-4">
          Departures
          <span v-if="selectedStop" class="text-gray-400 text-base font-normal">
            - Stop #{{ selectedStop.stopId }}
            <span v-if="currentStopName"> - {{ currentStopName }}</span>
          </span>
        </h2>

        <div v-if="!selectedStop" class="text-gray-400 text-center py-8">
          Select a stop from the list or map to see departures
        </div>

        <DepartureTable
          v-else
          :departures="stopDepartures"
          :is-loading="loadingDepartures"
        />
      </div>
    </div>
  </div>
</template>