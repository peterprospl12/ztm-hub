<script setup lang="ts">
import { ref, watch, onMounted, computed } from 'vue';
import { LMap, LTileLayer, LMarker, LPopup, LIcon } from '@vue-leaflet/vue-leaflet';
import 'leaflet/dist/leaflet.css';
import type { UserStop } from '../composables/useZtmData';
import type { PointExpression } from 'leaflet';
import apiClient from '../api/axios';

const props = defineProps<{
  userStops: UserStop[];
  selectedStopId?: number | null;
  mode: 'all' | 'favorites';
}>();

const emit = defineEmits<{
  selectStop: [stopId: number];
  selectStopForAdding: [stop: { stopId: number; stopName: string }];
}>();

interface StopLocation {
  stopId: number;
  stopName: string;
  lat: number;
  lng: number;
  isFavorite: boolean;
}

interface AllStop {
  id: number;
  name: string;
  code: string;
  lat: number;
  lon: number;
}

const allStopsData = ref<AllStop[]>([]);
const mapCenter = ref<[number, number]>([54.372158, 18.638306]); // Gdańsk
const zoom = ref(13);
const isLoading = ref(false);

const displayedStops = computed<StopLocation[]>(() => {
  const userStopIds = props.userStops.map(s => s.stopId);

  if (props.mode === 'favorites') {
    return allStopsData.value
      .filter(stop => userStopIds.includes(stop.id))
      .map(stop => ({
        stopId: stop.id,
        stopName: stop.name || `Stop ${stop.id}`,
        lat: stop.lat,
        lng: stop.lon,
        isFavorite: true,
      }));
  } else {
    // Wszystkie przystanki
    return allStopsData.value.map(stop => ({
      stopId: stop.id,
      stopName: stop.name || `Stop ${stop.id}`,
      lat: stop.lat,
      lng: stop.lon,
      isFavorite: userStopIds.includes(stop.id),
    }));
  }
});

async function fetchAllStops() {
  isLoading.value = true;
  try {
    const response = await apiClient.get('/Stops/all');
    allStopsData.value = response.data;
  } catch (e) {
    console.error('Failed to fetch stop locations:', e);
  } finally {
    isLoading.value = false;
  }
}

function handleMarkerClick(stop: StopLocation) {
  if (props.mode === 'all' && !stop.isFavorite) {
    emit('selectStopForAdding', { stopId: stop.stopId, stopName: stop.stopName });
  } else {
    emit('selectStop', stop.stopId);
  }
}

function getMarkerIcon(stop: StopLocation): string {
  if (props.selectedStopId === stop.stopId) {
    return 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png';
  }
  if (stop.isFavorite) {
    return 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png';
  }
  return 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-grey.png';
}

const defaultIconSize: PointExpression = [25, 41];
const defaultIconAnchor: PointExpression = [12, 41];

onMounted(() => {
  fetchAllStops();
});

watch(() => props.userStops, () => {
  if (props.mode === 'favorites' && props.userStops.length > 0) {
    const firstFavorite = displayedStops.value[0];
    if (firstFavorite) {
      mapCenter.value = [firstFavorite.lat, firstFavorite.lng];
      zoom.value = 14;
    }
  }
}, { deep: true });
</script>

<template>
  <div class="relative h-full min-h-[300px] rounded-lg overflow-hidden">
    <div v-if="isLoading" class="absolute inset-0 bg-gray-800/50 flex items-center justify-center z-[1000]">
      <span class="text-white">Loading map...</span>
    </div>

    <div v-if="displayedStops.length === 0 && !isLoading && mode === 'favorites'"
         class="absolute inset-0 flex items-center justify-center bg-gray-800 z-[1000]">
      <span class="text-gray-400">No favorite stops yet. Switch to "All Stops" to add some!</span>
    </div>

    <LMap
      v-model:zoom="zoom"
      :center="mapCenter"
      :use-global-leaflet="false"
      class="h-full w-full"
      style="min-height: 300px;"
    >
      <LTileLayer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        layer-type="base"
        name="OpenStreetMap"
      />

      <LMarker
        v-for="stop in displayedStops"
        :key="stop.stopId"
        :lat-lng="[stop.lat, stop.lng] as [number, number]"
        @click="handleMarkerClick(stop)"
      >
        <LIcon
          :icon-url="getMarkerIcon(stop)"
          :icon-size="defaultIconSize"
          :icon-anchor="defaultIconAnchor"
        />
        <LPopup>
          <div class="text-gray-900">
            <strong>{{ stop.stopName }}</strong>
            <br />
            <span class="text-sm text-gray-600">Stop #{{ stop.stopId }}</span>
            <br />
            <span v-if="stop.isFavorite" class="text-xs text-green-600 font-semibold">★ In your favorites</span>
            <span v-else class="text-xs text-gray-500">Click to add to favorites</span>
          </div>
        </LPopup>
      </LMarker>
    </LMap>

    <!-- Legend -->
    <div class="absolute bottom-2 left-2 bg-gray-900/90 text-white text-xs p-2 rounded z-[1000]">
      <div class="flex items-center gap-1 mb-1">
        <span class="w-3 h-3 bg-green-500 rounded-full"></span>
        <span>Your stops</span>
      </div>
      <div v-if="mode === 'all'" class="flex items-center gap-1">
        <span class="w-3 h-3 bg-gray-400 rounded-full"></span>
        <span>Available stops</span>
      </div>
    </div>
  </div>
</template>

<style>
.leaflet-default-icon-path {
  background-image: url("https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png");
}
</style>