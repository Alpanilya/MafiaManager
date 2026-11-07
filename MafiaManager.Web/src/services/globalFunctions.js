export function formatFunctions() {
  function formatDate(date, format) {
    if (!date) return

    const defaultFormat = {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    }

    const actualFormat = { ...defaultFormat, ...format }

    return new Date(date).toLocaleTimeString('ru-RU', actualFormat)
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

  return {
    formatDate,
    formatTime,
  }
}

export function numberFunctions() {
  function increaseValueBy(input, increase = 0) {
    if (isNaN(input) || isNaN(increase)) return input

    const num = Number(input)

    const inc = Number(increase)

    return num + inc
  }

  function validateInput(input, max, min = 0) {
    if (isNaN(input) || input <= min) return min

    if (input >= max) return max

    return input
  }

  return {
    increaseValueBy,
    validateInput,
  }
}

export function roleFunctions() {
  const { getId } = entityFunctions()

  function getRoleColor(roles, player) {
    const role = getPlayerRole(roles, player)

    return role?.color
  }

  function getPlayerRole(roles, player) {
    if (!roles || !Array.isArray(roles)) return

    return roles.find((r) => getId(r) === player.role)
  }

  function changePlayerRole(player, role) {
    if (!player || !role) {
      return
    }

    player.role = getId(role)
  }

  return {
    getRoleColor,
    getPlayerRole,
    changePlayerRole,
  }
}

export function entityFunctions() {
  function removeById(array, id) {
    if (!array || !id) {
      return
    }

    if (!Array.isArray(array)) {
      return
    }

    const index = array.findIndex((item) => getId(item) === id)

    if (index !== -1) {
      array.splice(index, 1)
    }
  }

  function getLastID(array) {
    if (!array) {
      return 0
    }

    if (!Array.isArray(array)) {
      return -1
    }

    if (array.length === 0) {
      return 0
    }

    return Math.max(...array.map((i) => getId(i)))
  }

  function getId(item) {
    if (!item) {
      return null
    }

    return item.id
  }

  function getName(item, defaultName = null) {
    if (!item) {
      return defaultName
    }

    return item.name ?? defaultName
  }

  function getById(array, id) {
    if (!array || !id) {
      return
    }

    if (!Array.isArray(array)) {
      return
    }

    return array.find((item) => getId(item) === id)
  }

  return {
    removeById,
    getLastID,
    getId,
    getName,
    getById,
  }
}

export function timingFunctions() {
  function debounce(fn, delay) {
    let timeout = null

    return (...args) => {
      if (timeout !== null) {
        clearTimeout(timeout)
      }

      timeout = setTimeout(() => {
        timeout = null
        fn(...args)
      }, delay)
    }
  }

  return {
    debounce,
  }
}
