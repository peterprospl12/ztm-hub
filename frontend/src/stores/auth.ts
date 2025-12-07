import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '../api/axios';
import router from '../router';

export const useAuthStore = defineStore('auth', () => {
    const token = ref<string | null>(localStorage.getItem('token') || null);

    const userEmail = computed(() => {
        if (!token.value) return null;
        try {
            // 1. Pobierz payload (druga część tokena)
            const base64Url = token.value.split('.')[1];
            if (!base64Url) return null;

            // 2. Zamień Base64Url na Base64
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');

            // 3. Zdekoduj
            const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            const payload = JSON.parse(jsonPayload);

            // 4. Szukaj emaila w różnych standardowych polach
            // 'email' - standardowe pole
            // 'unique_name' - często używane przez .NET Identity
            // 'sub' - czasem tu jest email, czasem ID
            return payload.email || payload.unique_name || payload.sub || 'User';
        } catch (e) {
            console.error('Failed to decode token:', e);
            return null;
        }
    });

    const isAuthenticated = computed(() => !!token.value);

    async function login(email: string, password: string) {
        try {
            const response = await apiClient.post('/Auth/login', { email, password });

            // Obsługa różnych formatów odpowiedzi (sam string lub obiekt)
            const receivedToken = typeof response.data === 'string'
                ? response.data
                : (response.data.token || response.data.value); // Czasem FluentResults zwraca w .value

            if (!receivedToken) throw new Error("No token received");

            token.value = receivedToken;
            localStorage.setItem('token', receivedToken);

            router.push('/dashboard');
            return true;
        } catch (error) {
            console.error('Login failed:', error);
            throw error;
        }
    }

    async function register(email: string, password: string) {
        try {
            await apiClient.post('/Auth/register', { email, password });
            return true;
        } catch (error) {
            console.error('Registration failed:', error);
            throw error;
        }
    }

    function logout() {
        token.value = null;
        localStorage.removeItem('token');
        router.push('/login');
    }

    return { token, userEmail, isAuthenticated, login, register, logout };
});