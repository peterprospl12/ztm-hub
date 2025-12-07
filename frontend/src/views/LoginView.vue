<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';

const email = ref('');
const password = ref('');
const authStore = useAuthStore();
const errorMessage = ref('');

const handleLogin = async () => {
  try {
    await authStore.login(email.value, password.value);
  } catch (error: any) {
    errorMessage.value = 'Login failed. Please check your credentials.';
  }
};
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-900 text-white">
    <div class="w-full max-w-md p-8 bg-gray-800 rounded-lg shadow-lg">
      <h2 class="text-2xl font-bold mb-6 text-center text-blue-400">Log in to ZtmHub</h2>

      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-sm font-medium mb-1">Email</label>
          <input v-model="email" type="email" required
            class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-blue-500 focus:outline-none" />
        </div>

        <div>
          <label class="block text-sm font-medium mb-1">Password</label>
          <input v-model="password" type="password" required
            class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-blue-500 focus:outline-none" />
        </div>

        <p v-if="errorMessage" class="text-red-500 text-sm">{{ errorMessage }}</p>

        <button type="submit"
          class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 rounded font-bold transition">
          Log In
        </button>
      </form>

      <p class="mt-4 text-center text-sm text-gray-400">
        Don't have an account?
        <router-link to="/register" class="text-blue-400 hover:underline">Sign up</router-link>
      </p>
    </div>
  </div>
</template>