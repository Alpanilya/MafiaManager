<script setup>
import { storeToRefs } from 'pinia'
import { entityFunctions } from '@/services/globalFunctions'
import { useGameSettings } from '@/stores/gameStore'

const { settings } = storeToRefs(useGameSettings())

const { getId, getName } = entityFunctions()
</script>

<template>
  <div id="roleList" class="mb-3">
    <label class="form-label text-white fw-semibold d-flex justify-content-between">
      <span>Список ролей:</span>
    </label>

    <div class="d-flex flex-column gap-1 mb-3 overflow-y-auto pe-1" style="max-height: 180px">
      <div
        v-for="role in settings.roles"
        :key="getId(role)"
        class="d-flex justify-content-between bg-dark p-2 rounded border border-secondary"
      >
        <div class="d-flex align-items-center gap-2">
          <span
            :style="{ backgroundColor: role.color }"
            class="rounded-circle flex-shrink-0"
            style="width: 12px; height: 12px; display: inline-block"
          ></span>

          <small class="fw-semibold text-white">{{ getName(role, 'Без роли') }}</small>
        </div>
      </div>

      <div v-if="settings.roles?.length === 0" class="text-secondary text-center p-2">
        <small>Роли не добавлены</small>
      </div>
    </div>
  </div>
</template>
