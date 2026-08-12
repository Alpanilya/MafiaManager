<script setup>
import { storeToRefs } from 'pinia'
import { useDefines, useGameStore } from '@/stores/gameStore'
import { formatFunctions, roleFunctions, entityFunctions } from '@/services/globalFunctions'
import { ref, watch, nextTick } from 'vue'

import arrowLeftIcon from '@/components/icons/arrowLeftIcon.vue'
import arrowRightIcon from '@/components/icons/arrowRightIcon.vue'
import playIcon from '@/components/icons/playIcon.vue'
import pauseIcon from '@/components/icons/pauseIcon.vue'
import resetIcon from '@/components/icons/resetIcon.vue'

const defines = useDefines()
const gameStore = useGameStore()

const {
  inEdit,
  gameState,
  players,
  roles,
  timeLeft,
  currentActiveRole,
  сurrentActivePlayer,
  gameLog,
} = storeToRefs(gameStore)

const {
  isSelectedRole,
  toggleTimer,
  resetTimer,
  getGameStats,
  toggleSelectPlayer,
  isSelectedPlayer,
  submitSelection,
} = gameStore

const { getRoleColor, getPlayerRole } = roleFunctions()

const { formatDate, formatTime } = formatFunctions()

const { getId, getName, getById } = entityFunctions()

const { playerStates, gamePhases } = storeToRefs(defines)

const logContainer = ref(null)

watch(
  () => gameLog.value.length,
  async () => {
    await nextTick()
    const el = logContainer.value
    if (el) el.scrollTop = el.scrollHeight
  },
)
</script>

<template>
  <div v-if="!inEdit" class="container-fluid bg-dark p-3 vh-100 overflow-hidden">
    <div class="row g-3">
      <div class="col-lg-3">
        <!--- Фаза -->
        <div
          class="card card-state"
          :style="{
            '--state-color': gamePhases[gameState?.phase]?.color,
            '--state-bg': gamePhases[gameState?.phase]?.bg,
            '--state-pos': gamePhases[gameState?.phase]?.pos,
          }"
        >
          <div class="card-body d-flex flex-column align-items-center rounded">
            <div class="d-flex flex-column align-items-start gap-1">
              <span class="fw-bold fs-5">Текущая фаза:</span>

              <div class="d-flex align-items-center gap-3">
                <span class="fw-bold fs-2">
                  {{ gamePhases[gameState?.phase]?.icon }}
                  {{ getName(gamePhases[gameState?.phase]) }}
                </span>

                <span class="badge border fs-3">
                  {{ gameState?.day }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!--- Роли -->
        <div class="card bg-dark flex-grow-0 border-0">
          <div class="card-body overflow-auto d-flex flex-column gap-2">
            <div v-for="role in roles.filter((r) => r.phase === 1)" :key="getId(role)">
              <div
                class="card p-2 role-card"
                :style="{ '--role-color': role.color }"
                :class="{ 'is-selected': isSelectedRole(role) }"
              >
                <div class="d-flex align-items-center justify-content-between px-2">
                  <div class="d-flex">
                    <span class="fw-bold" style="opacity: 0.8">Ход:</span>
                    <span class="fw-bold px-2"> {{ getName(role) }} </span>
                  </div>

                  <div v-if="isSelectedRole(role)" class="d-flex gap-3">
                    <div
                      class="font-monospace mb-0 fs-3"
                      :class="{ 'text-danger': timeLeft <= 10 }"
                    >
                      {{ formatTime(timeLeft) }}
                    </div>

                    <div class="d-flex btn-group gap-2">
                      <button v-if="!timerRunning" class="btn" @click="toggleTimer">
                        <playIcon />
                      </button>

                      <button v-if="timerRunning" class="btn" @click="toggleTimer">
                        <pauseIcon />
                      </button>

                      <button class="btn btn-sm fw-bold" @click="resetTimer">
                        <resetIcon />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!--- События -->
        <div class="card d-flex flex-column bg-dark border-0" style="height: 55vh">
          <div class="card-header fw-bold text-uppercase text-secondary border-0">
            📜 Сводка событий
          </div>
          <div ref="logContainer" class="card-body overflow-auto">
            <div v-for="log in gameLog" :key="getId(log)" class="mb-3 pb-2 border-bottom">
              <div class="d-flex flex-column">
                <span class="text-secondary small">{{ formatDate(log.date) }}</span>
                <br />
                <span class="text-light" v-html="log.message"></span>
              </div>
            </div>

            <div v-if="gameLog.length === 0" class="text-light text-center mt-4">
              Событий пока нет...
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-9">
        <div class="card bg-dark">
          <div class="card-header">
            <div class="flex-grow-1 text-white">
              <div
                v-if="gameState?.phase === 0 && gameLog.length > 0"
                class="d-flex align-items-stretch h-100"
              >
                <button class="btn d-flex align-items-center text-white">
                  <arrowLeftIcon />
                </button>

                <div class="d-flex flex-column px-2">
                  <small class="fw-bold">Выставление на голосование</small>
                  <h5 class="fw-bold mb-0">
                    Речь игрока: {{ getName(getById(players, сurrentActivePlayer)) }}
                  </h5>
                </div>

                <button class="btn d-flex align-items-center">
                  <arrowRightIcon />
                </button>
              </div>

              <h5 v-if="gameState?.phase === 1" class="mb-0 fw-bold text-white">
                Ходит: {{ getName(getById(roles, currentActiveRole)) }}
              </h5>

              <h5 v-if="gameState?.phase === 3" class="mb-0 fw-bold text-white p-3">
                Знакомство с участниками игры
              </h5>

              <h5 v-if="gameState?.phase === 2" class="mb-0 fw-bold text-white p-3">
                Голосование за исключение игрока из игры
              </h5>

              <h5 v-if="gameState?.phase === 4" class="mb-0 fw-bold text-white p-3">
                Игра завершена!
              </h5>
            </div>
          </div>

          <div class="card-body overflow-auto">
            <div class="row row-cols-lg-6 g-3">
              <div v-for="player in players" :key="getId(player)" class="col">
                <div
                  class="card player-card"
                  @click="toggleSelectPlayer(player)"
                  :style="{
                    '--role-color': getRoleColor(roles, player),
                  }"
                  :class="{
                    'is-selected': isSelectedPlayer(player),
                    'is-dead': player.state === 1,
                  }"
                >
                  <div
                    v-if="isSelectedPlayer(player)"
                    class="card-header d-flex justify-content-center fw-bold"
                  >
                    Выбран
                  </div>
                  <div class="card-body d-flex flex-column align-items-center p-3">
                    <div class="fw-bold mb-1 d-flex align-items-center gap-1">
                      <div v-if="player.state !== 0">
                        {{ playerStates[player.state]?.icon }}
                      </div>
                      <small class="mt-2 fw-bold">
                        {{ getName(getPlayerRole(roles, player)) }}
                      </small>
                    </div>

                    <span class="fs-2 fw-bold">{{ getId(player) }}</span>

                    <small class="text-uppercase">
                      {{ getName(player) }}
                    </small>
                  </div>
                  <div
                    v-if="player.state === 2"
                    class="card-footer d-flex justify-content-center fw-bold"
                  >
                    Номинирован
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="card-footer">
            <div class="d-flex justify-content-between align-items-center">
              <div class="d-flex gap-2">
                <span
                  v-for="group in getGameStats()"
                  :key="getName(group)"
                  class="badge badge-stats"
                  :style="{ '--stats-color': group.color }"
                >
                  {{ getName(group) }}: {{ group.alive }} / {{ group.total }}
                </span>
              </div>
              <button class="btn btn-success" @click="submitSelection">Далее</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style src="@/css/gameBoard.css"></style>
