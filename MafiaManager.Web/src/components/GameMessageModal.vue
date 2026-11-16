<script setup>
import { useGameStore } from '@/stores/gameStore'

import { storeToRefs } from 'pinia'

const gameStore = useGameStore()

const { summary } = storeToRefs(gameStore)

const { closeSummary } = gameStore
</script>

<template>
  <div
    v-if="summary != null"
    class="modal fade show d-block"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content bg-dark text-white border-secondary shadow-lg">
        <div class="modal-header border-secondary">
          <h5 class="modal-title fw-bold">📜 Итоги фазы</h5>
        </div>

        <div class="modal-body">
          <span class="text-light" v-html="summary.message"></span>
        </div>

        <div class="modal-footer border-secondary overflow-y-auto">
          <button
            type="button"
            class="btn btn-danger btn-text-contrast w-100 fw-bold py-2"
            @click="closeSummary"
          >
            Продолжить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
