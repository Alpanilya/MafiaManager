<script setup>
import { gameService } from './services/gameService'
import SettingsModal from '@/components/SettingsModal.vue'

const { game, inEdit, onGameCreated } = gameService()
</script>

<style>
body,
html {
  background-color: #212529;
}
</style>

<template>
  <SettingsModal :isOpen="inEdit" @created="onGameCreated" />
  <main class="container"></main>
</template>

<style scoped></style>
