import { createApp } from 'vue'
import { createPinia } from 'pinia'

import { useDefines } from '@/stores/gameStore'

import App from './components/App.vue'
import router from './router'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

const app = createApp(App)

app.use(createPinia())
app.use(router)

const defines = useDefines()

try {
  await useDefines().initialize()
} catch (e) {
  console.error('Не удалось загрузить справочники', e)
}

app.mount('#app')
