<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const authStore = useAuthStore();
const router = useRouter();
const errorMessage = ref('');

const handleRegister = async () => {
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match.';
    return;
  }

  try {
    await authStore.register(email.value, password.value);
    router.push('/login');
  } catch (error: any) {
    errorMessage.value = 'Registration failed. Email might be taken.';
  }
};
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-900 text-white">
    <div class="w-full max-w-md p-8 bg-gray-800 rounded-lg shadow-lg">
      <h2 class="text-2xl font-bold mb-6 text-center text-green-400">Create Account</h2>

      <form @submit.prevent="handleRegister" class="space-y-4">
        <div>
          <label class="block text-sm font-medium mb-1">Email</label>
          <input v-model="email" type="email" required
            class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-green-500 focus:outline-none" />
        </div>

        <div>
          <label class="block text-sm font-medium mb-1">Password</label>
          <input v-model="password" type="password" required
            class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-green-500 focus:outline-none" />
        </div>

        <div>
          <label class="block text-sm font-medium mb-1">Confirm Password</label>
          <input v-model="confirmPassword" type="password" required
            class="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-green-500 focus:outline-none" />
        </div>

        <p v-if="errorMessage" class="text-red-500 text-sm">{{ errorMessage }}</p>

        <button type="submit"
          class="w-full py-2 px-4 bg-green-600 hover:bg-green-700 rounded font-bold transition">
          Sign Up
        </button>
      </form>

      <p class="mt-4 text-center text-sm text-gray-400">
        Already have an account?
        <router-link to="/login" class="text-green-400 hover:underline">Log in</router-link>
      </p>
    </div>
  </div>
</template>