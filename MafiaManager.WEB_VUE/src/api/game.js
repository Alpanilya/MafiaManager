async function loadSettings() {
  const response = await fetch('/api/game/settings')

  if (!response.ok) {
    throw new Error(`Ошибка загрузки настроек: ${response.statusText}`)
  }

  return await response.json()
}

export { loadSettings }
