import { ref, watch, computed } from 'vue'
import { loadSettings } from '@/api/game'
import { entityService } from '@/services/entityService'

export function playerService(settingsRef, playersCountRef) {
  const service = entityService()
  const { roles, defaultRole } = roleService(settingsRef)
  const players = computed(() => {
    return service.getProperty(settingsRef.value, 'Players', 'players') ?? []
  })

  if (playersCountRef) {
    watch(
      playersCountRef,
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
  }

  function getRoleColor(player) {
    const role = getPlayerRole(player)

    return role?.color
  }

  function getPlayerRole(player) {
    return service.getProperty(player, 'Role', 'role')
  }

  function appendPlayers(count) {
    for (let i = 1; i <= count; i++) {
      appendPlayer()
    }
  }

  function appendPlayer() {
    const id = service.getLastID(players.value) + 1

    const player = {
      State: 0,
      ID: id,
      Name: `Игрок ${id}`,
      Role: defaultRole.value,
    }

    players.value.push(player)
  }

  function removePlayers(count) {
    for (let i = 1; i <= count; i++) {
      removeLastPlayer()
    }
  }

  function removeLastPlayer() {
    const id = service.getLastID(players.value)

    service.removeById(players.value, id)
  }

  function changePlayerRole(player, role) {
    if (!player || !role) {
      return
    }

    player.role = role
  }

  function getPlayerState(player) {
    return service.getProperty(player, 'State', 'state')
  }

  return {
    players,
    roles,
    getRoleColor,
    getPlayerRole,
    changePlayerRole,
    getPlayerState,
    ...service,
  }
}

export function roleService(settingsRef) {
  const service = entityService()
  const defaultColor = '#ff0000'
  const newRoleName = ref('')
  const newRoleColor = ref(defaultColor)
  const defaultRole = computed(() => {
    return service.getProperty(settingsRef.value, 'DefaultRole', 'defaultRole')
  })
  const roles = computed(() => {
    return service.getProperty(settingsRef.value, 'Roles', 'roles') ?? []
  })

  function appendCustomRole() {
    const name = newRoleName.value.trim()
    if (!name) return

    const nextId = service.getLastID(roles.value) + 1

    const role = {
      id: nextId,
      name: name,
      color: newRoleColor.value,
    }

    roles.value.push(role)

    newRoleName.value = ''
  }

  return {
    roles,
    newRoleName,
    newRoleColor,
    defaultRole,
    appendCustomRole,
    ...service,
  }
}

export function settingsService(isOpen, emit) {
  const settings = ref({})
  const isLoading = ref(false)

  watch(
    isOpen,
    async (newVal) => {
      if (newVal) {
        await getDefaultSettings()
      }
    },
    { immediate: true },
  )

  async function getDefaultSettings() {
    isLoading.value = true
    try {
      settings.value = await loadSettings()
    } finally {
      isLoading.value = false
    }
  }

  async function startGame() {
    const currentSettings = settings.value

    if (currentSettings.timeDelay <= 0) {
      alert('Время на ходы должно быть больше 0')
      return
    }

    if (currentSettings.players.length <= 0) {
      alert('Должен быть хотя бы один игрок')
      return
    }

    if (currentSettings.roles?.length === 0) {
      alert('Должна быть хотя бы одна роль')
      return
    }

    emit('created', settings.value)
  }

  return {
    settings,
    isLoading,
    startGame,
  }
}
