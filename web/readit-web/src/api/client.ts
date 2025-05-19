import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7281', // URL da API
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;