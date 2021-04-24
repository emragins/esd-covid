<template>
  <div id="app">
    <h1>COVID-19 Data</h1>
    <span>Countries with no confirmed cases are not shown.</span>

    <section v-if="errored">
      <p>
        We're sorry, we're not able to retrieve this information at the moment,
        please try back later
      </p>
    </section>

    <section v-if="loading">Loading...</section>

    <section v-else>
      <b-table
        striped
        hover
        :fields="tableFields"
        :items="tableData"
        :sort-by.sync="sortBy"
        :sort-desc.sync="sortDesc"
      >
        <!-- <template #cell(confirmed)="data">
          {{ data ? Number(data.unformatted).toLocaleString() : '' }}
        </template> -->

        <!-- <template #cell()="data">
          <i>{{ data }}</i>
        </template> -->
      </b-table>

      <div
        v-for="country in countryData"
        class="country"
        v-bind:key="country.code"
      >
        Name: {{ country.name }}:
        <span class="lighten">
          <span v-html="country.code"></span>
          {{ country.latest_data }}
        </span>
      </div>
    </section>
  </div>
</template>

<script>
import { HttpCovid } from "../lib/http";
export default {
  name: "Home",
  data() {
    return {
      sortBy: "confirmed",
      sortDesc: true,
      loading: true,
      countryData: [],
      tableData: [],
      tableFields: [
        // // A virtual column that doesn't exist in items
        // "index",
        // // A column that needs custom formatting
        // { key: "name", label: "Full Name" },
        // A regular column
        { key: "name", sortable: true },
        {
          key: "confirmed",
          sortable: true,
          formatter: (v) => Number(v).toLocaleString(),
        },
        // A regular column
        { key: "death_rate", sortable: true },
        // // A virtual column made up from two fields
        // { key: "nameage", label: "First name and age" },
      ],
      errored: false,
    };
  },
  methods: {
    loadCountries: function () {
      HttpCovid.get("/countries")
        .then((response) => {
          this.loading = false;
          this.countryData = response.data.data;
          let data = this.countryData
            .filter((cd) => cd.latest_data.confirmed > 0)
            .map((cd) => {
              return {
                name: cd.name,
                confirmed: cd.latest_data.confirmed,
                death_rate: cd.latest_data.calculated.death_rate
                  ? cd.latest_data.calculated.death_rate.toFixed(2)
                  : "--",
              };
            });

          this.tableData = data;
          //console.log('this.countryData', this.countryData);
        })
        .catch((e) => {
          this.errored = true;
          console.log(e);
        });
    },
  },
  mounted() {
    this.loadCountries();
  },
};
</script>

<style scoped>
.error {
  color: red;
}
</style>