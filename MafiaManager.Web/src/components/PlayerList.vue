<script setup>
import { storeToRefs } from 'pinia'
import { entityFunctions, roleFunctions } from '@/services/globalFunctions'
import { useGameSettings, usePlayerList } from '@/stores/gameStore'

const { settings } = storeToRefs(useGameSettings())

const playerList = usePlayerList()

const { getId, getName } = entityFunctions()

const { players } = storeToRefs(playerList)

const { getRoleColor, getPlayerRole, changePlayerRole } = roleFunctions()
</script>

<template>
  <div class="form-label text-white fw-semibold d-flex justify-content-between">
    <span for="playerGrid" class="form-label fw-semibold">Распределение ролей: </span>
  </div>
  <div class="row row-cols-2 row-cols-sm-3 g-3" id="playerGrid">
    <div v-for="player in players" :key="getId(player)" class="col">
      <div
        class="card text-center p-2 d-flex flex-column justify-content-between player-card"
        :style="{
          '--role-color': getRoleColor(settings.roles, player),
        }"
      >
        <div>
          <div class="display-6 fw-bold">{{ getId(player) }}</div>
          <small class="text-uppercase mt-1 mb-2 d-block">{{ getName(player) }}</small>
        </div>
        <button
          class="btn btn-sm dropdown-toggle btn-contrast"
          type="button"
          data-bs-toggle="dropdown"
        >
          {{ getName(getPlayerRole(settings.roles, player)) }}
        </button>

        <ul class="dropdown-menu dropdown-menu-contrast">
          <li v-for="role in settings.roles" :key="getId(role)">
            <a
              class="dropdown-item dropdown-item-contrast"
              href="#"
              @click.prevent="changePlayerRole(player, role)"
            >
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
