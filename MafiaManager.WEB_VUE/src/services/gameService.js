import { computed, ref, watch, onUnmounted } from 'vue'
import { entityService } from '@/services/entityService'
import { playerService } from '@/services/settingsService'
import { createGame, getNextPhase } from '@/api/game'

export function gameService() {
  const service = entityService()
  const { getRoleColor, getPlayerRole, getPlayerState } = playerService()
  const inEdit = ref(true)
  const game = ref({})
  const gameSettings = computed(() => {
    return service.getProperty(game.value, 'GameSettings', 'gameSettings')
  })
  const players = computed(() => {
    return service.getProperty(gameSettings.value, 'Players', 'players')
  })
  const gameState = computed({
    get() {
      return service.getProperty(game.value, 'GameState', 'gameState')
    },
    set(value) {
      let state = service.getPropertyName(game.value, 'GameState', 'gameState')

      game.value[state] = value
    },
  })
  const gameDay = computed(() => {
    return game.value.Day ?? game.value.day
  })
  const gameLog = computed(() => {
    return game.value.Log ?? game.value.log
  })
  const defaultTimeDelay = computed(() => {
    return service.getProperty(gameSettings.value, 'TimeDelay', 'timeDelay') ?? 60
  })
  const timeLeft = ref(0)
  const timerRunning = ref(false)
  let timerInterval = null

  watch(
    defaultTimeDelay,
    (newValue) => {
      if (timerRunning.value) return

      timeLeft.value = newValue
    },
    { immediate: true },
  )

  async function onGameCreated(settings) {
    const currentGame = await createGame(settings)

    game.value = currentGame

    inEdit.value = false
  }

  function getGameStats() {
    if (inEdit.value) return

    const selectedPlayers = players.value

    const roleStats = {}

    for (const player of selectedPlayers) {
      const role = getPlayerRole(player)
      const roleName = service.getName(role)
      const playerState = getPlayerState(player)

      if (!roleStats[roleName]) {
        const color = getRoleColor(player)

        roleStats[roleName] = {
          name: roleName,
          total: 0,
          alive: 0,
          color: color,
        }
      }

      roleStats[roleName].total++

      if (playerState != 1) roleStats[roleName].alive++
    }

    return roleStats
  }

  async function nextPhase() {
    const newPhase = await getNextPhase(game.value)

    gameState.value = newPhase
  }

  function formatTime(seconds) {
    seconds = Number(seconds)

    if (!Number.isFinite(seconds) || seconds < 0) {
      return '00:00'
    }

    seconds = Math.floor(seconds)

    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60

    return `${String(minutes).padStart(2, '0')}:${String(remainingSeconds).padStart(2, '0')}`
  }

  function toggleTimer() {
    if (timerRunning.value) {
      stopTimer()
    } else {
      startTimer()
    }
  }

  function startTimer() {
    timerRunning.value = true

    timerInterval = setInterval(() => {
      if (timeLeft.value > 0) {
        timeLeft.value--
      } else {
        stopTimer()
      }
    }, 1_000)
  }

  function stopTimer() {
    timerRunning.value = false

    if (timerInterval) {
      clearInterval(timerInterval)
      timerInterval = null
    }
  }

  function resetTimer() {
    stopTimer()

    timeLeft.value = defaultTimeDelay.value
  }

  onUnmounted(() => {
    stopTimer()
  })

  return {
    game,
    players,
    inEdit,
    gameState,
    gameDay,
    gameLog,
    timeLeft,
    timerRunning,
    onGameCreated,
    getRoleColor,
    getPlayerRole,
    getPlayerState,
    getGameStats,
    nextPhase,
    formatTime,
    toggleTimer,
    resetTimer,
    ...service,
  }
}
