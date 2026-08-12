<script setup>
import { storeToRefs } from 'pinia'

import { useVote, useGameStore } from '@/stores/gameStore'
import { roleFunctions, entityFunctions } from '@/services/globalFunctions'
import arrowLeftIcon from '@/components/icons/arrowLeftIcon.vue'
import arrowRightIcon from '@/components/icons/arrowRightIcon.vue'

const voteStore = useVote()
const gameStore = useGameStore()

const { roles } = storeToRefs(gameStore)

const { nominatedPlayers, inVote } = storeToRefs(voteStore)

const { voteAgainst, voteFor, playerVoteStats, finishVote } = voteStore

const { getId, getName } = entityFunctions()

const { getRoleColor, getPlayerRole } = roleFunctions()
</script>

<template>
  <div
    v-if="inVote"
    class="modal fade show d-block"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content bg-dark text-white border-secondary shadow-lg">
        <div class="modal-header border-secondary">
          <h5 class="modal-title fw-bold">⚖️ Голосование</h5>
        </div>

        <div class="modal-body">
          <div class="row g-3">
            <div
              v-for="player in nominatedPlayers"
              :key="getId(player)"
              class="col-4 col-md-4 col-lg-4"
            >
              <div
                class="card player-card"
                :style="{
                  '--role-color': getRoleColor(roles, player),
                }"
              >
                <div class="card-body d-flex flex-column align-items-center p-3">
                  <div class="fw-bold mb-1 d-flex align-items-center gap-1">
                    <small class="mt-2 fw-bold">
                      {{ getName(getPlayerRole(roles, player)) }}
                    </small>
                  </div>

                  <span class="fs-2 fw-bold">{{ getId(player) }}</span>

                  <small class="text-uppercase">
                    {{ getName(player) }}
                  </small>
                </div>
                <div class="card-footer d-flex justify-content-center fw-bold gap-2">
                  <button
                    type="button"
                    class="btn btn-danger btn-sm fw-bold"
                    @click="voteAgainst(player)"
                  >
                    <arrowLeftIcon />
                  </button>
                  <span class="badge bg-secondary fs-4 px-3 py-1">{{
                    playerVoteStats(player)
                  }}</span>
                  <button
                    type="button"
                    class="btn btn-success btn-sm fw-bold"
                    @click="voteFor(player)"
                  >
                    <arrowRightIcon />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer border-secondary overflow-y-auto">
          <button type="button" class="btn btn-danger w-100 fw-bold py-2" @click="finishVote">
            Подвести итоги ⚖️
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
