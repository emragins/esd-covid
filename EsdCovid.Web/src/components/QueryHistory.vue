
<template>
  <section v-if="loading"><!--intentionally blank--></section>

  <section v-else>
    <div>
      <span class="queriesHeading">Top 5 Queries</span> 
      <p class="queriesText">{{ queries }}</p>
    </div>
  </section>
</template>

<script>
import { HttpQueries } from "../lib/http";
export default {
  name: "Home",
  data() {
    return { loading: true, queries: [] };
  },
  methods: {
    load() {
      HttpQueries.get("/CommonQueries", { params: { code: "public" } })
        .then((response) => {
          let formatted = response.data.map(
            (d) => `${d.QueryText} (${d.NumTimesHit})`
          );

          this.queries = formatted.join(" | ");
          this.loading = false;
        })
        .catch((e) => console.log(e));
    },
  },
  mounted() {
    this.load();
  },
};
</script>

<style scoped>
.queriesHeading {
  font-weight: bold;
}
.queriesText {
    margin-left: 10px;
    }
</style>