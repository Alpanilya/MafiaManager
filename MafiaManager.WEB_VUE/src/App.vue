<script setup>
import { ref } from 'vue'
import SettingsModal from '@/components/SettingsModal.vue'

const isModalOpen = ref(true)
</script>

<template>
  <main class="container">
    <SettingsModal :isOpen="isModalOpen" />
  </main>
</template>

<style scoped></style>
