<script setup>
import { storeToRefs } from 'pinia'
import { gameService } from './services/gameService'
import { gameStore } from '@/stores/gameStore'
import SettingsModal from '@/components/SettingsModal.vue'

const {
  inEdit,
  players,
  gameState,
  gameDay,
  gameLog,
  timeLeft,
  timerRunning,
  getId,
  getName,
  onGameCreated,
  getRoleColor,
  getPlayerRole,
  getPlayerState,
  getGameStats,
  formatDate,
  formatTime,
  nextPhase,
  toggleTimer,
  resetTimer,
} = gameService()

const gameStoreInstance = gameStore()

gameStoreInstance.instance()

const { playerStates, gameStates } = storeToRefs(gameStoreInstance)
</script>

<style>
body,
html {
  background-color: #212529;
}
</style>

<template>
  <SettingsModal :isOpen="inEdit" @created="onGameCreated" />
  <div v-if="!inEdit" class="container-fluid bg-dark text-white h-100 p-3 d-flex flex-column">
    <div class="row g-3 h-100">
      <div class="col-lg-3 col-md-4 d-flex flex-column">
        <div class="card bg-dark border-secondary border-2 text-center mb-3 text-white">
          <div :class="[gameStates[gameState]?.color, 'card-body']">
            <h2 class="fw-bold mb-0">
              {{ gameStates[gameState]?.icon }} {{ gameStates[gameState]?.name }}
            </h2>
            <span class="fs-5 fw-bold"> Круг {{ gameDay }} </span>
          </div>
        </div>

        <div class="card bg-dark border-secondary border-2 text-center mb-3 text-white">
          <div class="card-body">
            <h6 class="text-uppercase text-secondary fw-bold mb-1">Оставшееся время</h6>
            <div
              class="display-3 font-monospace fw-bold mb-3"
              :class="{ 'text-danger': timeLeft <= 10 }"
            >
              {{ formatTime(timeLeft) }}
            </div>
            <div class="btn-group gap-1">
              <button class="btn btn-primary btn-sm fw-bold" @click="toggleTimer">
                {{ !timerRunning ? '▶️ Старт' : '⏸ Пауза' }}
              </button>
              <button class="btn btn-danger btn-sm fw-bold" @click="resetTimer">🔄 Сброс</button>
            </div>
          </div>
        </div>

        <div class="card bg-dark border-secondary border-2 text-center mb-3 text-white">
          <div class="card-body">
            <div class="btn-group flex-column gap-2">
              <button class="btn btn-danger fw-bold py-2" @click="nextPhase()">
                Следующая фаза
              </button>

              <button v-if="gameStates[gameState]?.name === 'День'" class="btn btn-primary py-2">
                ⚖️ Начать голосование
              </button>

              <button v-if="gameStates[gameState]?.name === 'Ночь'" class="btn btn-primary py-2">
                🎲 Ход игрока
              </button>
            </div>
          </div>
        </div>

        <div class="card bg-dark border-secondary border-2 text-center mb-3 text-white flex-grow-1">
          <div class="card-header border-secondary fw-bold text-uppercase text-secondary">
            📜 Сводка событий
          </div>
          <div class="card-body overflow-auto">
            <div v-for="log in gameLog" :key="log" class="mb-2 pb-2 border-bottom border-secondary">
              <span class="text-secondary small">{{ formatDate(log.date) }}</span>
              <br />
              <span class="text-light"> {{ log.message }} </span>
            </div>

            <div v-if="gameLog.length === 0" class="text-light text-center mt-4">
              Событий пока нет...
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-9 col-md-8 d-flex flex-column">
        <div class="card bg-dark border-secondary border-2 text-center mb-3 text-white flex-grow-1">
          <div
            class="card-header border-secondary d-flex justify-content-between align-items-center"
          >
            <h5 class="mb-0 fw-bold text-white">👥 Игроки за столом</h5>

            <div class="d-flex gap-2 flex-wrap justify-content-end">
              <span
                v-for="group in getGameStats()"
                :key="group"
                class="badge"
                :style="{ backgroundColor: group.color }"
              >
                {{ group.name }}: {{ group.alive }} / {{ group.total }}
              </span>
            </div>
          </div>

          <div class="card-body overflow-auto p-4">
            <div class="row row-cols-2 row-cols-sm-3 g-3">
              <div v-for="player in players" :key="getId(player)" class="col">
                <div
                  class="card bg-dark text-white border-2 text-center flex-column mb-3 p-3"
                  :style="{ borderColor: getRoleColor(player) }"
                >
                  <div :class="[playerStates[getPlayerState(player)]?.color, 'fw-bold mb-1']">
                    {{ playerStates[getPlayerState(player)]?.icon }}
                    {{ playerStates[getPlayerState(player)]?.name }}
                  </div>
                  <div class="display-6 fw-bold">{{ getId(player) }}</div>

                  <small class="text-uppercase mt-1">{{ getName(player) }}</small>
                  <div class="badge mt-2" :style="{ backgroundColor: getRoleColor(player) }">
                    {{ getName(getPlayerRole(player), 'Без роли') }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <partial name="_Settings" />
  </div>
</template>

<style scoped></style>
