<script setup lang="ts">
import { ref } from 'vue';
import type { UserStop } from '../composables/useZtmData';

const props = defineProps<{
  stop: UserStop;
  isSelected?: boolean;
}>();

const emit = defineEmits<{
  select: [stop: UserStop];
  remove: [userStopId: string];
  update: [userStopId: string, displayName: string];
}>();

const isEditing = ref(false);
const editingName = ref('');

function startEditing() {
  isEditing.value = true;
  editingName.value = props.stop.displayName || '';
}

function saveEdit() {
  emit('update', props.stop.id, editingName.value);
  isEditing.value = false;
  editingName.value = '';
}

function cancelEdit() {
  isEditing.value = false;
  editingName.value = '';
}
</script>

<template>
  <li
    class="p-3 rounded transition"
    :class="isSelected ? 'bg-blue-600/30 border border-blue-500' : 'bg-gray-700 hover:bg-gray-600'"
  >
    <!-- Edit mode -->
    <div v-if="isEditing" class="space-y-2">
      <input
        v-model="editingName"
        type="text"
        class="w-full p-2 rounded bg-gray-600 border border-gray-500 focus:border-blue-500 focus:outline-none text-sm"
        placeholder="Enter custom name"
        @keyup.enter="saveEdit"
        @keyup.escape="cancelEdit"
      />
      <div class="flex gap-2">
        <button @click="saveEdit" class="flex-1 py-1 bg-green-600 hover:bg-green-700 rounded text-sm">
          Save
        </button>
        <button @click="cancelEdit" class="flex-1 py-1 bg-gray-600 hover:bg-gray-500 rounded text-sm">
          Cancel
        </button>
      </div>
    </div>

    <!-- Normal mode -->
    <div v-else class="flex items-center justify-between cursor-pointer" @click="emit('select', stop)">
      <div>
        <span class="font-medium">{{ stop.displayName || `Stop ${stop.stopId}` }}</span>
        <span class="text-gray-400 text-sm ml-2">#{{ stop.stopId }}</span>
      </div>
      <div class="flex gap-1">
        <button
          @click.stop="startEditing"
          class="text-blue-400 hover:text-blue-300 p-1"
          title="Edit stop"
        >
          ✎
        </button>
        <button
          @click.stop="emit('remove', stop.id)"
          class="text-red-400 hover:text-red-300 p-1"
          title="Remove stop"
        >
          ✕
        </button>
      </div>
    </div>
  </li>
</template>