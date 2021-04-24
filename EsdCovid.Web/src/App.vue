<template>
  <div id="app">
    <b-container>
      <router-view />
    </b-container>
  </div>
</template>

<script>
export default {
  name: "app",
};
</script>

<style>
#app {
  max-width: 600px;
}
</style>

