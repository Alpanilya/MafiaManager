export async function loadSettings() {
  const response = await fetch('/api/game/settings')

  if (!response.ok) {
    throw new Error(`Ошибка загрузки настроек: ${response.statusText}`)
  }

  return await response.json()
}

export async function createGame(settings) {
  const response = await fetch('/api/game/create', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(settings),
  })

  if (!response.ok) {
    throw new Error(`Ошибка создания игры: ${response.statusText}`)
  }

  return await response.json()
}
