import axios from "axios";

let baseURL;

if (!process.env.NODE_ENV || process.env.NODE_ENV === "development") {
  baseURL = "http://127.0.0.1/";
} else {
  baseURL = "http://api.example.com";
}

export const HttpCovid = axios.create({
  baseURL: 'https://corona-api.com',
});
