<script setup>
import { toRef, defineProps } from 'vue'
import { playerService } from '@/services/settingsService'

const props = defineProps({
  gameSettings: Object,
})

const playersCount = defineModel('playersCount', { type: Number })

const { players, roles, getId, getName, getRoleColor, getPlayerRole, changePlayerRole } =
  playerService(toRef(props, 'gameSettings'), playersCount)
</script>

<template>
  <div class="row row-cols-2 row-cols-sm-3 g-3" id="playersGrid">
    <div v-for="player in players" :key="getId(player)" class="col">
      <div
        class="card bg-dark text-white border-2 text-center p-2 h-100 d-flex flex-column justify-content-between"
        :style="{ borderColor: getRoleColor(player) }"
      >
        <div class="display-6 fw-bold">{{ getId(player) }}</div>

        <small class="text-uppercase mt-1">{{ getName(player) }}</small>
        <button
          class="btn btn-secondary btn-sm dropdown-toggle"
          type="button"
          data-bs-toggle="dropdown"
          aria-expanded="false"
        >
          {{ getName(getPlayerRole(player)) }}
        </button>
        <ul class="dropdown-menu dropdown-menu-dark">
          <li v-for="role in roles" :key="getId(role)">
            <a class="dropdown-item" href="#" @click="changePlayerRole(player, role)">
              {{ getName(role) }}
            </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
  <div v-if="players.length === 0" class="col-12 fw-bold text-secondary text-center p-3">
    <small>Укажите кол-во игроков</small>
  </div>
</template>
