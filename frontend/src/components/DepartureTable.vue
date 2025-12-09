<script setup lang="ts">
import type { Departure } from '../composables/useZtmData';

defineProps<{
  departures: Departure[];
  isLoading?: boolean;
}>();

function formatTime(isoTime: string): string {
  if (!isoTime) return '--:--';
  try {
    const date = new Date(isoTime);
    return date.toLocaleTimeString('pl-PL', { hour: '2-digit', minute: '2-digit' });
  } catch {
    return isoTime.substring(0, 5);
  }
}
</script>

<template>
  <div class="overflow-x-auto">
    <div v-if="isLoading" class="text-gray-400 text-center py-8">
      Loading departures...
    </div>

    <div v-else-if="departures.length === 0" class="text-gray-400 text-center py-8">
      No departures found for this stop
    </div>

    <table v-else class="w-full">
      <thead>
        <tr class="border-b border-gray-700 text-left text-gray-400">
          <th class="pb-2">Line</th>
          <th class="pb-2">Direction</th>
          <th class="pb-2">Scheduled</th>
          <th class="pb-2">Estimated</th>
          <th class="pb-2">Delay</th>
          <th class="pb-2">Status</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(dep, index) in departures"
          :key="index"
          class="border-b border-gray-700/50"
        >
          <td class="py-3">
            <span class="bg-blue-600 px-2 py-1 rounded font-bold text-sm">
              {{ dep.line }}
            </span>
          </td>
          <td class="py-3">{{ dep.direction }}</td>
          <td class="py-3 text-gray-400">{{ dep.scheduledTime }}</td>
          <td class="py-3 font-semibold">{{ formatTime(dep.estimatedTime) }}</td>
          <td class="py-3 font-semibold">
            <span v-delay-color="dep.delaySeconds">
              {{ $ztm.formatDelay(dep.delaySeconds) }}
            </span>
          </td>
          <td class="py-3">
            <span
              class="text-xs px-2 py-1 rounded"
              :class="dep.status === 'REALTIME' ? 'bg-green-600/30 text-green-400' : 'bg-gray-600/30 text-gray-400'"
            >
              {{ dep.status === 'REALTIME' ? 'Live' : 'Scheduled' }}
            </span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>