import { ref, watch } from 'vue'
import { loadSettings } from '@/api/game'
import { entityService } from '@/services/entityService'

export function playerService(settingsRef, playersCountRef) {
  const service = entityService()
  const { roles, defaultRole } = roleService(settingsRef)
  const players = ref([])
  const playersCount = playersCountRef

  if (playersCountRef) {
    watch(
      playersCountRef,
      (newVal) => {
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

  watch(
    settingsRef,
    (newVal) => {
      if (newVal) {
        initRefs(settingsRef.value)
      }
    },
    { immediate: true },
  )

  function initRefs(settings) {
    if (!settings) {
      return
    }

    const rawPlayers = settings.players ?? settings.Players

    players.value = rawPlayers ?? []

    if (playersCount) {
      playersCount.value = players.value.length
    }
  }

  function getRoleColor(player) {
    const role = getPlayerRole(player)

    return role?.color
  }

  function getPlayerRole(player) {
    return player.role ?? player.Role ?? null
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

  return {
    players,
    roles,
    getRoleColor,
    getPlayerRole,
    changePlayerRole,
    ...service,
  }
}

export function roleService(settingsRef) {
  const service = entityService()
  const defaultColor = '#ff0000'
  const roles = ref([])
  const newRoleName = ref('')
  const newRoleColor = ref(defaultColor)
  const defaultRole = ref('')

  watch(
    settingsRef,
    (newVal) => {
      if (newVal) {
        initRefs(newVal)
      }
    },
    { immediate: true },
  )

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

  function initRefs(settings) {
    if (!settings) {
      return
    }

    const rawRoles = settings.roles ?? settings.Roles

    roles.value = rawRoles ?? []

    defaultRole.value = settings.defaultRole ?? settings.DefaultRole ?? null
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

export function initSettingsModal(isOpen) {
  const settings = ref({})

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
    settings.value = await loadSettings()
  }

  return {
    settings,
  }
}
