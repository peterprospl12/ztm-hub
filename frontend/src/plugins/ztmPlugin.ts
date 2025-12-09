import type { App } from 'vue';
import { vDelayColor } from '../directives/vDelayColor';

export const ZtmPlugin = {
    install(app: App) {
        app.directive('delay-color', vDelayColor);

        app.config.globalProperties.$ztm = {
            formatDelay(seconds: number | null | undefined): string {
                if (seconds == null || seconds === 0) return 'On time';
                if (seconds < 0) {
                    const absSeconds = Math.abs(seconds);
                    if (absSeconds < 60) return `${absSeconds}s early`;
                    const minutes = Math.floor(absSeconds / 60);
                    return `${minutes}m early`;
                }

                if (seconds < 60) return `+${seconds}s`;
                const minutes = Math.floor(seconds / 60);
                const remainingSeconds = seconds % 60;
                if (remainingSeconds === 0) return `+${minutes}m`;
                return `+${minutes}m ${remainingSeconds}s`;
            },

            formatTime(time: string | null | undefined): string {
                if (!time) return '--:--';
                return time.substring(0, 5);
            },
        };
    },
};

declare module 'vue' {
    interface ComponentCustomProperties {
        $ztm: {
            formatDelay(seconds: number | null | undefined): string;
            formatTime(time: string | null | undefined): string;
        };
    }
}

export default ZtmPlugin;