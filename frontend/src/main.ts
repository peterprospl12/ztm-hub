import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import './style.css'
import App from './App.vue'
import ZtmPlugin from './plugins/ztmPlugin'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(ZtmPlugin)

app.mount('#app')