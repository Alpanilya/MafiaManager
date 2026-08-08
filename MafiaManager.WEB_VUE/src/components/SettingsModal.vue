<script setup>
import { toRef, defineProps, defineModel } from 'vue'
import { settingsService } from '@/services/settingsService'

import RolesList from '@/components/RolesList.vue'
import PlayerList from '@/components/PlayerList.vue'

const props = defineProps({
  isOpen: Boolean,
})

const emit = defineEmits(['created'])

const playersCount = defineModel('playersCount', { type: Number, default: 0 })

const { settings, startGame } = settingsService(toRef(props, 'isOpen'), emit)
</script>

<template>
  <div
    v-if="isOpen"
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
            <label for="playerCount" class="form-label fw-semibold"> Количество игроков: </label>

            <input
              type="number"
              class="form-control bg-body-tertiary border-secondary"
              id="playerCount"
              v-model.number="playersCount"
              @input="playersCount = Math.min(100, playersCount)"
              min="0"
              max="100"
              required
            />
          </div>

          <div class="mb-3">
            <label for="timeDelay" class="form-label fw-semibold">Время на ходы (сек) </label>

            <input
              type="number"
              class="form-control bg-body-tertiary border-secondary"
              id="timeDelay"
              v-model.number="settings.timeDelay"
              @input="settings.timeDelay = Math.min(3600, settings.timeDelay)"
              min="0"
              max="3600"
              required
            />
          </div>

          <RolesList :gameSettings="settings" />

          <PlayerList :gameSettings="settings" v-model:playersCount="playersCount" />
        </div>

        <div class="modal-footer border-secondary overflow-y-auto">
          <button type="button" class="btn btn-danger w-100 fw-bold py-2" @click="startGame">
            Начать игру
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
