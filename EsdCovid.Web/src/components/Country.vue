<template>
  <div>
    <div class="back"><b-button v-on:click="goHome">Go Back to All Countries</b-button></div>
    <div class="heading">{{ data.name }} Statistics</div>
    <div class="group">
      <span class="label">Population</span>
      <span class="value">{{ formatNumber(data.population) }}</span>
    </div>

    <div class="subheading">Rollups</div>

    <div class="group">
      <span class="label">Cases per Million Population</span>
      <span class="value">{{
        formatNumber(data.latest_data.calculated.cases_per_million_population)
      }}</span>
    </div>
    <div class="group">
      <span class="label">Death Rate</span>
      <span class="value">{{
        formatNumber(data.latest_data.calculated.death_rate)
      }}</span>
    </div>
    <div class="group">
      <span class="label">Recovered vs Death Ratio</span>
      <span class="value">{{
        formatNumber(data.latest_data.calculated.recovered_vs_death_ratio)
      }}</span>
    </div>
    <div class="group">
      <span class="label">Recovery Rate</span>
      <span class="value">{{
        formatNumber(data.latest_data.calculated.recovery_rate)
      }}</span>
    </div>

    <div class="subheading">Case Counts</div>
    <div class="group">
      <span class="label">Confirmed</span>
      <span class="value">{{ formatNumber(data.latest_data.confirmed) }}</span>
    </div>
    <div class="group">
      <span class="label">Critical</span>
      <span class="value">{{ formatNumber(data.latest_data.critical) }}</span>
    </div>
    <div class="group">
      <span class="label">Deaths</span>
      <span class="value">{{ formatNumber(data.latest_data.deaths) }}</span>
    </div>
    <div class="group">
      <span class="label">Recovered</span>
      <span class="value">{{ formatNumber(data.latest_data.recovered) }}</span>
    </div>

    <!-- <div class="group">
          <span class="label">timeline</span>
      <span class="value">{{data.timeline}}</span>: (455) [{…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, {…}, …]
        </div> -->
    <!-- <div class="group">
          <span class="label">today</span>
      <span class="value">{{data.today}}</span>: {deaths: 539, confirmed: 39273}
        </div> -->
    <div class="updatedat">
      <span>Updated At {{ new Date(data.updated_at).toLocaleString() }}</span>
    </div>
  </div>
</template>

<script>
import { HttpCovid } from "../lib/http";
export default {
  name: "Country",
  data() {
    return {
      loading: true,
      data: null,
    };
  },
  methods: {
    formatNumber(num) {
      return num ? Number(num).toLocaleString() : "--";
    },
    goHome() {
      this.$router.push({ path: "/" });
    },
    loadCountry(countryCode) {
      HttpCovid.get(`/countries/${countryCode}`)
        .then((response) => {
          this.loading = false;
          console.log(response.data);
          this.data = response.data.data;
        })
        .catch((e) => {
          this.errored = true;
          console.log(e);
        });
    },
  },
  mounted() {
    console.log("route.params", this.$route.params);
    this.loadCountry(this.$route.params.countryCode);
  },
};
</script>

<style scoped>
.back{
align-content: center;
padding: 10px;
}
.heading {
  font-size: 1.5em;
}
.subheading {
  margin-top: 10px;
  font-size: 1.3em;
}
.group {
  background-color: #f9f9f9;
  display: grid;
  grid-template-columns: auto 1fr;
  padding: 8px 16px;
  margin: 0px 5px;
  margin-bottom: 1px;
}
.label {
  font-weight: bold;
}
.value {
  justify-self: right;
}
.updatedat {
  margin-top: 10px;
  font-size: 0.9em;
}
</style>