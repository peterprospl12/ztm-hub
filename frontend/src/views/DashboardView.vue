<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useZtmData, type Departure, type UserStop } from '../composables/useZtmData';
import StopCard from '../components/StopCard.vue';
import DepartureTable from '../components/DepartureTable.vue';
import StopMap from '../components/StopMap.vue';

const authStore = useAuthStore();
const { userStops, currentStopName, isLoading, error, fetchUserStops, fetchDepartures, addStop, removeStop, updateStop } = useZtmData();

// Map mode
const mapMode = ref<'all' | 'favorites'>('favorites');

// Form state for adding stop
const stopToAdd = ref<{ stopId: number; stopName: string } | null>(null);
const customStopName = ref('');
const isAddingStop = ref(false);

// Selected stop for departures
const selectedStop = ref<UserStop | null>(null);
const stopDepartures = ref<Departure[]>([]);
const loadingDepartures = ref(false);

onMounted(() => {
  fetchUserStops();
});

// Obsługa wyboru przystanku do dodania (z mapy w trybie "all")
function handleSelectStopForAdding(stop: { stopId: number; stopName: string }) {
  stopToAdd.value = stop;
  customStopName.value = '';
}

// Dodaj przystanek
async function handleAddStop() {
  if (!stopToAdd.value) return;

  isAddingStop.value = true;
  await addStop(stopToAdd.value.stopId, customStopName.value || undefined);
  stopToAdd.value = null;
  customStopName.value = '';
  isAddingStop.value = false;
}

// Anuluj dodawanie
function cancelAddStop() {
  stopToAdd.value = null;
  customStopName.value = '';
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

// Obsługa kliknięcia na marker na mapie (ulubiony przystanek)
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

      <!-- Top row: Your Stops + Map -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
        <!-- Left column: Your Stops + Add form -->
        <div class="lg:col-span-1 space-y-6">
          <!-- Add Stop Panel (pokazuje się gdy wybierzemy przystanek na mapie) -->
          <div v-if="stopToAdd" class="bg-green-900/30 border border-green-600 rounded-lg p-4">
            <h2 class="text-xl font-semibold mb-3 text-green-400">Add Stop</h2>
            <div class="mb-3">
              <p class="text-sm text-gray-300">Selected stop:</p>
              <p class="font-semibold">{{ stopToAdd.stopName }} <span class="text-gray-400">#{{ stopToAdd.stopId }}</span></p>
            </div>
            <div class="mb-3">
              <label class="block text-sm text-gray-400 mb-1">Custom Name (optional)</label>
              <input
                v-model="customStopName"
                type="text"
                :placeholder="stopToAdd.stopName"
                class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-green-500 focus:outline-none"
              />
            </div>
            <div class="flex gap-2">
              <button
                @click="handleAddStop"
                :disabled="isAddingStop"
                class="flex-1 py-2 bg-green-600 hover:bg-green-700 disabled:bg-green-800 rounded font-semibold transition"
              >
                {{ isAddingStop ? 'Adding...' : 'Add to Favorites' }}
              </button>
              <button
                @click="cancelAddStop"
                class="px-4 py-2 bg-gray-600 hover:bg-gray-500 rounded transition"
              >
                Cancel
              </button>
            </div>
          </div>

          <!-- Stops list -->
          <div class="bg-gray-800 rounded-lg p-4">
            <div class="flex justify-between items-center mb-4">
              <h2 class="text-xl font-semibold">Your Stops</h2>
              <span v-if="userStops.length > 0" class="text-sm text-gray-400">
                {{ userStops.length }} stop{{ userStops.length !== 1 ? 's' : '' }}
              </span>
            </div>

            <div v-if="isLoading && userStops.length === 0" class="text-gray-400 text-center py-4">
              Loading...
            </div>

            <div v-else-if="userStops.length === 0" class="text-gray-400 text-center py-4">
              <p>No stops added yet.</p>
              <p class="text-sm mt-2">Switch to "All Stops" mode and click on a stop to add it!</p>
            </div>

            <ul v-else class="space-y-2 max-h-[450px] overflow-y-auto pr-1 custom-scrollbar">
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
            <!-- Map header with mode toggle -->
            <div class="flex justify-between items-center mb-4">
              <h2 class="text-xl font-semibold">Map</h2>
              <div class="flex bg-gray-700 rounded-lg p-1">
                <button
                  @click="mapMode = 'favorites'"
                  :class="[
                    'px-3 py-1 rounded text-sm font-medium transition',
                    mapMode === 'favorites'
                      ? 'bg-blue-600 text-white'
                      : 'text-gray-400 hover:text-white'
                  ]"
                >
                  My Stops
                </button>
                <button
                  @click="mapMode = 'all'"
                  :class="[
                    'px-3 py-1 rounded text-sm font-medium transition',
                    mapMode === 'all'
                      ? 'bg-blue-600 text-white'
                      : 'text-gray-400 hover:text-white'
                  ]"
                >
                  All Stops
                </button>
              </div>
            </div>

            <div class="h-[450px]">
              <StopMap
                :user-stops="userStops"
                :selected-stop-id="selectedStop?.stopId"
                :mode="mapMode"
                @select-stop="handleMapSelectStop"
                @select-stop-for-adding="handleSelectStopForAdding"
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

<style scoped>
/* Custom scrollbar for the stops list */
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: #374151;
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #6b7280;
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #9ca3af;
}
</style>