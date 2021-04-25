
<template>
  <section v-if="loading"><!--intentionally blank--></section>

  <section v-else>
    <div>
      <h3>Common Queries</h3>
      <p>{{ queries }}</p>
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
