<script setup>
import { toRef, defineProps } from 'vue'
import { roleService } from '@/services/settingsModalService'

const props = defineProps({
  gameSettings: Object,
})

const { roles, newRoleName, newRoleColor, appendCustomRole, getId, getName, removeById } =
  roleService(toRef(props, 'gameSettings'))
</script>

<template>
  <div id="roleList" class="mb-3">
    <label class="form-label text-white fw-semibold d-flex justify-content-between">
      <span>Список ролей</span>

      <small class="text-secondary" id="rolesCount"> Всего: {{ roles.length }} </small>
    </label>

    <div class="d-flex flex-column gap-1 mb-3 overflow-y-auto pe-1" style="max-height: 180px">
      <div
        v-for="role in roles"
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

        <button
          type="button"
          class="btn-close btn-close-white btn-sm"
          @click="removeById(roles, getId(role))"
        ></button>
      </div>

      <div v-if="roles.length === 0" class="text-secondary text-center p-2">
        <small>Роли не добавлены</small>
      </div>
    </div>

    <div class="input-group border-secondary">
      <input type="text" v-model="newRoleName" class="form-control" placeholder="Новая роль..." />

      <input
        type="color"
        class="form-control form-control-color w-25 flex-grow-0"
        v-model="newRoleColor"
      />

      <button class="btn btn-success flex-grow-0" type="button" @click="appendCustomRole">+</button>
    </div>
  </div>
</template>
