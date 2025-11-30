// src/api/http.ts
import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5204";

export const api = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor simples p/ log (ajuda a debugar)
api.interceptors.request.use((config) => {
  console.log("[API] Request:", config.method?.toUpperCase(), config.url, config.params, config.data);
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("[API] Error:", error.response?.status, error.response?.data);
    return Promise.reject(error);
  }
);
