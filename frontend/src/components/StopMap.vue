<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { LMap, LTileLayer, LMarker, LPopup, LIcon } from '@vue-leaflet/vue-leaflet';
import 'leaflet/dist/leaflet.css';
import type { UserStop } from '../composables/useZtmData';
import type { PointExpression } from 'leaflet';
import apiClient from '../api/axios';

const props = defineProps<{
  userStops: UserStop[];
  selectedStopId?: number | null;
}>();

const emit = defineEmits<{
  selectStop: [stopId: number];
}>();

interface StopLocation {
  stopId: number;
  stopName: string;
  lat: number;
  lng: number;
}

// Format z API /api/Stops/all
interface AllStop {
  id: number;
  name: string;
  code: string;
  lat: number;
  lon: number;
}

const stopLocations = ref<StopLocation[]>([]);
const mapCenter = ref<[number, number]>([54.372158, 18.638306]); // Gdańsk
const zoom = ref(13);
const isLoading = ref(false);

async function fetchStopLocations() {
  if (props.userStops.length === 0) {
    stopLocations.value = [];
    return;
  }

  isLoading.value = true;
  try {
    const response = await apiClient.get('/Stops/all');
    const allStops: AllStop[] = response.data;

    // Filtruj tylko przystanki użytkownika
    const userStopIds = props.userStops.map(s => s.stopId);
    const locations: StopLocation[] = [];

    for (const stop of allStops) {
      if (userStopIds.includes(stop.id)) {
        locations.push({
          stopId: stop.id,
          stopName: stop.name || `Stop ${stop.id}`,
          lat: stop.lat,
          lng: stop.lon, // API zwraca 'lon', nie 'lng'
        });
      }
    }

    stopLocations.value = locations;

    // Wycentruj mapę na pierwszym przystanku
    if (locations.length > 0 && locations[0]) {
      mapCenter.value = [locations[0].lat, locations[0].lng];
      zoom.value = 14;
    }
  } catch (e) {
    console.error('Failed to fetch stop locations:', e);
  } finally {
    isLoading.value = false;
  }
}

function handleMarkerClick(stopId: number) {
  emit('selectStop', stopId);
}

const defaultIconSize: PointExpression = [25, 41];
const defaultIconAnchor: PointExpression = [12, 41];

onMounted(() => {
  fetchStopLocations();
});

watch(() => props.userStops, () => {
  fetchStopLocations();
}, { deep: true });
</script>

<template>
  <div class="relative h-full min-h-[300px] rounded-lg overflow-hidden">
    <div v-if="isLoading" class="absolute inset-0 bg-gray-800/50 flex items-center justify-center z-[1000]">
      <span class="text-white">Loading map...</span>
    </div>

    <div v-if="stopLocations.length === 0 && !isLoading" class="absolute inset-0 flex items-center justify-center bg-gray-800 z-[1000]">
      <span class="text-gray-400">Add stops to see them on the map</span>
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
        v-for="stop in stopLocations"
        :key="stop.stopId"
        :lat-lng="[stop.lat, stop.lng] as [number, number]"
        @click="handleMarkerClick(stop.stopId)"
      >
        <LIcon
          :icon-url="selectedStopId === stop.stopId
            ? 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png'
            : 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png'"
          :icon-size="defaultIconSize"
          :icon-anchor="defaultIconAnchor"
        />
        <LPopup>
          <div class="text-gray-900">
            <strong>{{ stop.stopName }}</strong>
            <br />
            <span class="text-sm text-gray-600">Stop #{{ stop.stopId }}</span>
          </div>
        </LPopup>
      </LMarker>
    </LMap>
  </div>
</template>

<style>
.leaflet-default-icon-path {
  background-image: url("https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png");
}
</style>