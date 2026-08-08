import { ref } from 'vue'
import { entityService } from '@/services/entityService'
import { createGame } from '@/api/game'

export function gameService() {
  const inEdit = ref(true)
  const game = ref({})

  async function onGameCreated(settings) {
    console.log(JSON.stringify(settings), 'onGameCreated')

    const currentGame = await createGame(settings)

    game.value = currentGame

    inEdit.value = false
  }

  return {
    game,
    inEdit,
    onGameCreated,
    ...entityService,
  }
}
