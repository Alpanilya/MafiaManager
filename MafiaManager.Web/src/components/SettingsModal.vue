<script setup>
import { useGameSettings } from '@/stores/gameStore'

import RolesList from '@/components/RolesList.vue'
import PlayerList from '@/components/PlayerList.vue'

import { storeToRefs } from 'pinia'

const emit = defineEmits(['created'])

const gameSettings = useGameSettings()

const { inEdit, playersCount, settings, timeDelayInput } = storeToRefs(gameSettings)

const { validateSettings, changePlayers, changeTimeDelay, checkPlayersInput, checkTimeDelay } =
  gameSettings

function startGame() {
  const error = validateSettings()

  if (error) {
    alert(error)
    return
  }

  emit('created', settings.value)
}
</script>

<template>
  <div
    v-if="inEdit"
    class="modal fade show d-block"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content bg-dark text-white border-secondary shadow-lg">
        <div class="modal-header border-secondary">
          <h5 class="modal-title fw-bold">⚙️ Настройки новой игры</h5>
        </div>

        <div class="modal-body">
          <div class="mb-3">
            <label for="playerCount" class="form-label fw-semibold">Количество игроков:</label>

            <input
              type="number"
              inputmode="numeric"
              pattern="[0-9]*"
              class="form-control bg-body-tertiary border-secondary mb-3"
              id="playerCount"
              v-model.number="playersCount"
              @input="checkPlayersInput(playersCount)"
              required
            />

            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changePlayers(-1)"
            >
              -1
            </button>
            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changePlayers(-5)"
            >
              -5
            </button>
            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changePlayers(-10)"
            >
              -10
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changePlayers(1)"
            >
              +1
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changePlayers(5)"
            >
              +5
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changePlayers(10)"
            >
              +10
            </button>
          </div>

          <div class="mb-3">
            <label for="timeDelay" class="form-label fw-semibold">Время на ходы (сек):</label>

            <input
              type="number"
              inputmode="numeric"
              pattern="[0-9]*"
              class="form-control bg-body-tertiary border-secondary mb-3"
              id="timeDelay"
              v-model.number="timeDelayInput"
              @input="checkTimeDelay(timeDelayInput, 3600)"
              required
            />

            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changeTimeDelay(-1)"
            >
              -1
            </button>
            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changeTimeDelay(-10)"
            >
              -10
            </button>
            <button
              type="button"
              class="btn btn-danger fw-bold mb-3 me-2"
              @click="changeTimeDelay(-60)"
            >
              -60
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changeTimeDelay(1)"
            >
              +1
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changeTimeDelay(10)"
            >
              +10
            </button>
            <button
              type="button"
              class="btn btn-success fw-bold mb-3 me-2"
              @click="changeTimeDelay(60)"
            >
              +60
            </button>
          </div>

          <RolesList :gameSettings="settings" />

          <PlayerList />
        </div>

        <div class="modal-footer border-secondary overflow-y-auto">
          <button
            type="button"
            class="btn btn-danger btn-text-contrast w-100 fw-bold py-2"
            @click="startGame"
          >
            Начать игру
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
