<template>
  <div id="app">
    <h1>COVID-19 Data</h1>

    <section v-if="errored">
      <p class="error">
        We're sorry, we're not able to retrieve this information at the moment,
        please try back later
      </p>
    </section>

    <section v-if="loading">Loading...</section>

    <section v-else>
      <span class="subtext">
        * Countries with no confirmed cases are not shown.
      </span>
      <autocomplete
        :search="search"
        @submit="onAutocompleteSubmit"
      ></autocomplete>
      <b-table
        striped
        hover
        :fields="tableFields"
        :items="tableData"
        :sort-by.sync="sortBy"
        :sort-desc.sync="sortDesc"
      >
        <template v-slot:cell(name)="{ item, value }">
          <b-link v-on:click="navigateToCountryCode(item.code)">
            {{ value }}</b-link
          >
        </template>

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
import { HttpCovid, HttpQueries } from "../lib/http";
import Autocomplete from "@trevoreyre/autocomplete-vue";
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
  components: {
    Autocomplete,
  },
  methods: {
    navigateToCountryCode: function (countryCode) {
      this.$router.push({ name: "country", params: { countryCode } });
    },
    loadCountries: function () {
      HttpCovid.get("/countries")
        .then((response) => {
          this.loading = false;
          let data = response.data.data
            .filter((cd) => cd.latest_data.confirmed > 0)
            .map((cd) => {
              return {
                name: cd.name,
                code: cd.code,
                confirmed: cd.latest_data.confirmed,
                death_rate: cd.latest_data.calculated.death_rate
                  ? cd.latest_data.calculated.death_rate.toFixed(2)
                  : "--",
              };
            });

          this.countryData = data;
          this.tableData = data;
          //console.log('this.countryData', this.countryData);
        })
        .catch((e) => {
          this.errored = true;
          console.log(e);
        });
    },
    search(input) {
      if (!input || input.length < 2) return [];

      if (this.countryData)
        return this.countryData
          .filter((cd) => {
            return cd.name.toLowerCase().startsWith(input.toLowerCase());
          })
          .map((cd) => cd.name);
    },
    onAutocompleteSubmit(value) {
      // value will be undefined if nothing was selected
      if (!value) return;
      console.log(value);
      var toCountry = this.countryData.filter((cd) => {
        return cd.name.toLowerCase().startsWith(value.toLowerCase());
      });
      if (toCountry.length > 0) {
        this.navigateToCountryCode(toCountry[0].code);
      }
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
.subtext {
  font-style: italic;
}
</style>