async function getResponse(url, request) {
  const response = await fetch(url, request)

  if (!response.ok) {
    throw new Error(`Ошибка загрузки настроек: ${response.statusText}`)
  }

  return await response.json()
}

export async function loadSettings() {
  return await getResponse('/api/game/settings')
}

export async function createGame(settings) {
  return await getResponse('/api/game/create', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(settings),
  })
}

export async function getPlayerStates() {
  return await getResponse('/api/game/playerStates')
}

export async function getGamePhases() {
  return await getResponse('/api/game/gamePhases')
}

export async function getNextPhase(game) {
  return await getResponse('/api/game/getNextPhase', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(game),
  })
}

export async function getVoteResult(game, votedPlayers) {
  const body = {
    sessionID: game.ID,
    gameState: game.gameState,
    votedPlayers: votedPlayers,
  }

  return await getResponse('/api/game/getVoteResult', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export async function load(sessionID) {
  return await getResponse(`/api/game/load?sessionID=${sessionID}`)
}
