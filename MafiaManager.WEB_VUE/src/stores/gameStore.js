import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { getPlayerStates, getGameStates } from '@/api/game'

const PLAYERSTATE_CONFIGS = {
  0: { name: 'Живой', color: 'text-success', icon: '🟢' },
  1: { name: 'Мертвый', color: 'text-danger', icon: '💀' },
  2: { name: 'Защищенный', color: 'text-primary', icon: '🛡️' },
}

const GAMESTATE_CONFIGS = {
  0: { name: 'День', color: 'bg-info', icon: '☀️' },
  1: { name: 'Ночь', color: 'bg-dark', icon: '🌙' },
}

export const gameStore = defineStore('game', () => {
  const playerStates = ref({})
  const gameStates = ref({})

  async function instance() {
    await intanceGameStates()
    await intancePlayerStates()
  }

  async function intancePlayerStates() {
    const states = await getPlayerStates()

    for (const stateId of states) {
      const config = PLAYERSTATE_CONFIGS[stateId]

      if (!config) {
        throw new Error(
          `GameStore - Неизвестное состояние игрока: ID ${stateId}. Добавьте обработку в PLAYERSTATE_CONFIGS.`,
        )
      }
    }

    playerStates.value = PLAYERSTATE_CONFIGS
  }

  async function intanceGameStates() {
    const states = await getGameStates()

    for (const stateId of states) {
      const config = GAMESTATE_CONFIGS[stateId]

      if (!config) {
        throw new Error(
          `GameStore - Неизвестное состояние игрока: ID ${stateId}. Добавьте обработку в GAMESTATE_CONFIGS.`,
        )
      }
    }

    gameStates.value = GAMESTATE_CONFIGS
  }

  return {
    playerStates,
    gameStates,
    instance,
  }
})
