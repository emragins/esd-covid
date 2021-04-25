import axios from "axios";

export const HttpCovid = axios.create({
  baseURL: "https://corona-api.com",
});

export const HttpQueries = axios.create({
  baseURL: "https://esdcovid-functions.azurewebsites.net/api/",
});
