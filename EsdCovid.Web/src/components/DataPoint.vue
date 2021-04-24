<template>
  <div class="group">
    <span class="label">{{ label }}</span>
    <span class="value">{{ value }}</span>
  </div>
</template>

<style scoped>
.group {
  display: block;
}
.label {
  font-weight: bold;
}
.value {
  justify-self: right;
}
</style>