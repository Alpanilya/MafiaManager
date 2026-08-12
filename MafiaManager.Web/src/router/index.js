import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [{ 
    path: '/game/:sessionID', 
    name: 'GameBoard', 
    component: () => import('@/components/GameBoard.vue') 
  }],
})

export default router
