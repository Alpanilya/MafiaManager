import * as signalR from '@microsoft/signalr'
import { defineStore, storeToRefs } from 'pinia'
import { ref, readonly, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {
  numberFunctions,
  entityFunctions,
  roleFunctions,
  timingFunctions,
} from '@/services/globalFunctions'

import {
  getPlayerStates,
  getGamePhases,
  createGame,
  getNextPhase,
  getVoteResult,
  load,
  loadSettings,
} from '@/api/game'

import daySkyBg from '@/components/icons/daySkyBg.svg?url'
import nightSkyBg from '@/components/icons/nightSkyBg.svg?url'
import sunsetSkyBg from '@/components/icons/sunsetSkyBg.svg?url'
import dayBreakSky from '@/components/icons/dayBreakSkyBg.svg?url'
import gameOver from '@/components/icons/gameOver.svg?url'

const PLAYERSTATE_CONFIGS = {
  0: { name: 'Живой', color: 'text-success', icon: '🟢' },
  1: { name: 'Мертвый', color: 'text-danger', icon: '💀' },
  2: { name: 'Номинирован', color: 'text-success', icon: '✅' },
}

const GAMEPHASE_CONFIGS = {
  0: {
    name: 'День',
    color: '#add8e6',
    bg: `url("${daySkyBg}")`,
    pos: '75% 45%',
  },
  1: {
    name: 'Ночь',
    color: '#000000',
    bg: `url("${nightSkyBg}")`,
    pos: 'center',
  },
  2: {
    name: 'День',
    color: '#ff6b35',
    bg: `url("${sunsetSkyBg}")`,
    pos: 'center',
  },
  3: {
    name: 'День',
    color: '#f9c23d',
    bg: `url("${dayBreakSky}")`,
    pos: '75% 45%',
  },
  4: {
    bg: `url("${gameOver}")`,
    color: 'transparent',
    pos: 'center',
  },
}

export const useDefines = defineStore('defines', () => {
  const playerStates = ref({})
  const gamePhases = ref({})
  const isReady = ref(false)

  async function initialize() {
    if (isReady.value) return

    await Promise.all([initializeGamePhases(), initializePlayerStates()])

    isReady.value = true
  }

  async function initializePlayerStates() {
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

  async function initializeGamePhases() {
    const states = await getGamePhases()

    for (const stateId of states) {
      const config = GAMEPHASE_CONFIGS[stateId]

      if (!config) {
        throw new Error(
          `GameStore - Неизвестное состояние игрока: ID ${stateId}. Добавьте обработку в GAMEPHASE_CONFIGS.`,
        )
      }
    }

    gamePhases.value = GAMEPHASE_CONFIGS
  }

  return {
    playerStates: readonly(playerStates),
    gamePhases: readonly(gamePhases),
    initialize,
  }
})

export const useGameStore = defineStore('game', () => {
  const { getPlayerRole } = roleFunctions()
  const { getId, getName } = entityFunctions()
  const { increaseValueBy, validateInput } = numberFunctions()
  const { debounce } = timingFunctions()

  const router = useRouter()
  const route = useRoute()

  const inEdit = ref(false)
  const game = ref({})
  const timeLeft = ref(0)
  const timerRunning = ref(false)
  const isVoted = ref(false)
  const voteStarted = ref(false)
  const isSubmitting = ref(false)

  const gameSettings = computed(() => {
    return game.value.gameSettings ?? {}
  })
  const roles = computed(() => {
    return gameSettings.value.roles ?? []
  })
  const players = computed(() => {
    return gameSettings.value.players ?? []
  })
  const timeDelay = computed(() => {
    return gameSettings.value.timeDelay ?? 0
  })
  const gameState = computed(() => {
    return game.value.gameState
  })
  const gameLog = computed(() => {
    return game.value.log || []
  })
  const id = computed(() => {
    return game.value.id
  })
  const currentActiveRole = computed(() => {
    return game.value.currentActiveRole
  })
  const сurrentActivePlayer = computed(() => {
    return game.value.currentActivePlayer
  })
  const actions = computed(() => {
    return game.value.actions || []
  })
  const cursor = ref(0)

  const connection = new signalR.HubConnectionBuilder()
    .withUrl('/notificationRelay')
    .withAutomaticReconnect()
    .build()

  connection.on('OnNewLogMessage', (newMessage) => {
    if (!game.value.log) game.value.log = []

    gameLog.value.push(newMessage)
  })

  connection.on('OnLogUpdated', (updated) => {
    const i = gameLog.value.findIndex((l) => l.id === updated.id)

    if (i !== -1) gameLog.value[i] = updated
  })

  connection.on('OnGameUpdated', (newGame) => {
    game.value = newGame
    isSubmitting.value = false
  })

  connection.on('OnRejectedAction', (response) => {
    isSubmitting.value = false
    alert(`Действие отклонено: ${response.reason}`)
  })

  let timerInterval = null

  watch(
    () => route.params.sessionID,
    async (newValue) => {
      await initSession(newValue)
    },
    { immediate: true },
  )

  watch(
    timeDelay,
    (newValue) => {
      if (timerRunning.value) return

      timeLeft.value = newValue
    },
    { immediate: true },
  )

  async function onGameCreated(settings) {
    const currentGame = await createGame(settings)

    game.value = currentGame

    await connection.start()
    await connection.invoke('Join', id.value)

    inEdit.value = false

    router.push(`/game/${currentGame.id}`)
  }

  function getGameStats() {
    if (inEdit.value) return

    const selectedPlayers = players.value

    const roleStats = {}

    for (const player of selectedPlayers) {
      const role = getPlayerRole(roles.value, player)

      const roleName = getName(role)
      const playerState = player.state

      if (!roleStats[roleName]) {
        const color = role?.color

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
    const isConfirmed = confirm('Вы уверены, что хотите пропустить голосование?')

    if (isConfirmed) {
      isVoted.value = true
    }

    if (!isVoted.value) {
      alert('Завершите голосование в текущей фазе')
      return
    }

    const newGame = await getNextPhase(game.value)

    game.value = newGame

    isVoted.value = false
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

    timeLeft.value = timeDelay.value
  }

  function toggleSelectPlayer(player) {
    debouncedToggleSelect(player)
  }

  async function onToggleSelectPlayer(player) {
    if (isSubmitting.value) return

    isSubmitting.value = true

    try {
      await connection.invoke('ToggleSelectPlayer', id.value, {
        playerID: getId(player),
      })
    } catch (error) {
      alert(`${error.message}`)
    } finally {
      isSubmitting.value = false
    }
  }

  async function voteStart() {
    /*if (selectedPlayers.value.length === 0) {
      alert('Должен быть выбран хотя бы 1 претендент')
      return
    }*/

    voteStarted.value = true
  }

  async function onVoteCompleted(playersVoted) {
    const response = await getVoteResult(game.value, playersVoted)

    game.value = response

    isVoted.value = true

    voteStarted.value = false

    //selectedPlayers.value = []
  }

  async function initSession(currentSessionID) {
    if (!currentSessionID) {
      inEdit.value = true
      return
    }

    inEdit.value = false

    try {
      const data = await load(currentSessionID)

      if (data) {
        game.value = data

        if (connection.state === 'Disconnected') {
          await connection.start()
        }
        await connection.invoke('Join', currentSessionID)
      } else {
        game.value = {}
        inEdit.value = true
      }
    } catch (error) {
      alert('Не удалось загрузить игру!')
      game.value = {}
      inEdit.value = true
    }
  }

  function isSelectedRole(role) {
    return getId(role) === currentActiveRole.value
  }

  async function submitSelection() {
    if (isSubmitting.value) return

    isSubmitting.value = true

    try {
      await connection.invoke('SubmitAction', id.value)
    } catch (error) {
      alert(`${error.message}`)
    } finally {
      isSubmitting.value = false
    }
  }

  async function submitVoteResult(voteResult) {
    if (isSubmitting.value) return

    isSubmitting.value = true

    try {
      await connection.invoke('SubmitVoteAction', id.value, { votes: voteResult })
    } catch (error) {
      alert(`${error.message}`)
    } finally {
      isSubmitting.value = false
    }
  }

  async function switchAction() {
    if (isSubmitting.value) return

    isSubmitting.value = true

    try {
      await connection.invoke('SwitchGameAction', id.value, {
        cursor: cursor.value,
      })
    } catch (error) {
      alert(`${error.message}`)
    } finally {
      isSubmitting.value = false
      cursor.value = 0
    }
  }

  function addValue(modelRef, value, maxValue) {
    const result = increaseValueBy(modelRef.value, value)

    modelRef.value = validateInput(result, maxValue, players.value.length * -1)
  }

  function changeCursor(value) {
    addValue(cursor, value, actions.value.length - 1)

    debouncedSwitchAction()
  }

  const debouncedSwitchAction = debounce(switchAction, 200)

  const debouncedToggleSelect = debounce(onToggleSelectPlayer, 200)

  return {
    gameState,
    gameLog,
    players,
    roles,
    currentActiveRole,
    сurrentActivePlayer,
    inEdit: readonly(inEdit),
    timerRunning: readonly(timerRunning),
    timeLeft: readonly(timeLeft),
    voteStarted: readonly(voteStarted),
    isSubmitting: readonly(isSubmitting),
    isSelectedRole,
    toggleTimer,
    resetTimer,
    getGameStats,
    toggleSelectPlayer,
    submitSelection,
    onGameCreated,
    onVoteCompleted,
    submitVoteResult,
    changeCursor,
  }
})

export const useGameSettings = defineStore('gameSettings', () => {
  const { inEdit } = storeToRefs(useGameStore())
  const { increaseValueBy, validateInput } = numberFunctions()

  const settings = ref({})
  const isLoading = ref(false)
  const timeDelayInput = ref(0)
  const playersCount = ref(0)

  const timeDelay = computed({
    get() {
      return settings.value.timeDelay
    },
    set(value) {
      settings.value.timeDelay = value
    },
  })

  watch(
    inEdit,
    async (newVal) => {
      if (newVal) {
        await getDefaultSettings()
      }
    },
    { immediate: true },
  )

  watch(
    timeDelayInput,
    (newVal) => {
      timeDelay.value = newVal
    },
    { immediate: true },
  )

  async function getDefaultSettings() {
    isLoading.value = true
    try {
      settings.value = await loadSettings()
      timeDelayInput.value = timeDelay.value
    } finally {
      isLoading.value = false
    }
  }

  function addValue(modelRef, value, maxValue) {
    const result = increaseValueBy(modelRef.value, value)

    modelRef.value = validateInput(result, maxValue)
  }

  function changePlayers(value) {
    addValue(playersCount, value, 100)
  }

  function checkPlayersInput(value) {
    playersCount.value = validateInput(value, 100)
  }

  function changeTimeDelay(value) {
    addValue(timeDelayInput, value, 3600)
  }

  function checkTimeDelay(value) {
    timeDelayInput.value = validateInput(value, 3600)
  }

  function validateSettings() {
    if (timeDelay.value <= 0) {
      return 'Время на ходы должно быть больше 0'
    }

    if (settings.value.players.length <= 0) {
      return 'Должен быть хотя бы один игрок'
    }

    if (settings.value.roles?.length === 0) {
      return 'Должна быть хотя бы одна роль'
    }

    return null
  }

  return {
    timeDelayInput,
    playersCount,
    settings,
    isLoading: readonly(isLoading),
    inEdit: readonly(inEdit),
    changePlayers,
    changeTimeDelay,
    checkPlayersInput,
    checkTimeDelay,
    validateSettings,
  }
})

export const usePlayerList = defineStore('playerList', () => {
  const { settings, playersCount } = storeToRefs(useGameSettings())

  const { getLastID, removeById } = entityFunctions()
  const players = computed(() => {
    return settings.value?.players ?? []
  })

  const defaultRole = computed(() => {
    return settings.value?.defaultRole
  })

  watch(
    playersCount,
    (newVal) => {
      if (!players.value) return

      const diff = newVal - players.value.length

      if (diff > 0) {
        appendPlayers(diff)
      } else {
        removePlayers(Math.abs(diff))
      }
    },
    { immediate: true },
  )

  function appendPlayers(count) {
    for (let i = 1; i <= count; i++) {
      appendPlayer()
    }
  }

  function appendPlayer() {
    const id = getLastID(players.value) + 1

    const player = {
      state: 0,
      id: id,
      name: `Игрок ${id}`,
      role: defaultRole.value,
    }

    players.value.push(player)
  }

  function removePlayers(count) {
    for (let i = 1; i <= count; i++) {
      removeLastPlayer()
    }
  }

  function removeLastPlayer() {
    const id = getLastID(players.value)

    removeById(players.value, id)
  }

  return {
    players,
  }
})

export const useVote = defineStore('vote', () => {
  const gameStore = useGameStore()
  const { gameState, players } = storeToRefs(gameStore)
  const { getId } = entityFunctions()
  const { submitVoteResult } = gameStore

  const nominatedPlayers = computed(() => {
    if (!players.value) return []

    return players.value.filter((player) => player.state === 2)
  })

  const alivePlayers = computed(() => players.value?.filter((player) => player.state !== 1) ?? [])
  const maxVote = computed(() => alivePlayers.value.length)

  const inVote = computed(() => {
    return gameState.value?.phase === 2
  })

  watch(inVote, (newVal) => {
    if (!newVal) {
      votedCount.value = 0
      voteResult.value = {}
    }
  })

  const voteResult = ref({})

  const votedCount = ref(0)

  function voteAgainst(player) {
    const id = getId(player)

    const current = voteResult.value[id] ?? 0

    if (current === 0) {
      return
    }

    voteResult.value = { ...voteResult.value, [id]: current - 1 }
    votedCount.value--
  }

  function voteFor(player) {
    if (votedCount.value >= maxVote.value) return

    const id = getId(player)

    const current = voteResult.value[id] ?? 0

    voteResult.value = { ...voteResult.value, [id]: current + 1 }
    votedCount.value++
  }

  async function finishVote() {
    await submitVoteResult(voteResult.value)

    inVote.value = false
  }

  function playerVoteStats(player) {
    const id = getId(player)

    return voteResult.value[id] ?? 0
  }

  return {
    nominatedPlayers,
    inVote,
    voteAgainst,
    voteFor,
    playerVoteStats,
    finishVote,
  }
})
