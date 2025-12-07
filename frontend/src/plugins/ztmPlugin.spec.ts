import { describe, it, expect, beforeEach } from 'vitest';
import { createApp } from 'vue';
import { ZtmPlugin } from './ztmPlugin';

describe('ZtmPlugin', () => {
    let $ztm: {
        formatDelay(seconds: number | null | undefined): string;
        formatTime(time: string | null | undefined): string;
    };

    beforeEach(() => {
        const app = createApp({});
        app.use(ZtmPlugin);
        $ztm = app.config.globalProperties.$ztm;
    });

    describe('formatDelay', () => {
        it('should return "On time" for 0 seconds', () => {
            expect($ztm.formatDelay(0)).toBe('On time');
        });

        it('should return "On time" for null', () => {
            expect($ztm.formatDelay(null)).toBe('On time');
        });

        it('should return "On time" for undefined', () => {
            expect($ztm.formatDelay(undefined)).toBe('On time');
        });

        it('should format early arrival correctly', () => {
            expect($ztm.formatDelay(-30)).toBe('30s early');
            expect($ztm.formatDelay(-125)).toBe('2m early');
        });

        it('should format delay in seconds correctly', () => {
            expect($ztm.formatDelay(30)).toBe('+30s');
            expect($ztm.formatDelay(59)).toBe('+59s');
        });

        it('should format delay in minutes correctly', () => {
            expect($ztm.formatDelay(60)).toBe('+1m');
            expect($ztm.formatDelay(120)).toBe('+2m');
            expect($ztm.formatDelay(90)).toBe('+1m 30s');
        });
    });

    describe('formatTime', () => {
        it('should return "--:--" for null', () => {
            expect($ztm.formatTime(null)).toBe('--:--');
        });

        it('should return "--:--" for undefined', () => {
            expect($ztm.formatTime(undefined)).toBe('--:--');
        });

        it('should return "--:--" for empty string', () => {
            expect($ztm.formatTime('')).toBe('--:--');
        });

        it('should format time correctly', () => {
            expect($ztm.formatTime('22:38:00')).toBe('22:38');
            expect($ztm.formatTime('09:05')).toBe('09:05');
        });
    });
});