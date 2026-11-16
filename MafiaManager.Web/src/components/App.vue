<script setup>
import { useGameStore } from '@/stores/gameStore'
import SettingsModal from '@/components/SettingsModal.vue'
import VoteModal from '@/components/PlayerVoteModal.vue'
import GameBoard from './GameBoard.vue'
import GameMessageModal from './GameMessageModal.vue'

const gameStore = useGameStore()

const { onGameCreated } = gameStore
</script>

<style>
@import '@/css/gameBoard.css';

body,
html {
  background-color: #212529;
}
</style>

<template>
  <SettingsModal @created="onGameCreated" />
  <VoteModal />
  <GameBoard />
  <GameMessageModal />
</template>
