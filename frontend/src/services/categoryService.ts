import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL + '/Categories/';

const getCategories = () => {
  return axios.get(API_URL, { headers: authHeader() });
};

const createCategory = (name: string) => {
  return axios.post(API_URL, { name }, { headers: authHeader() });
};

const deleteCategory = (id: number) => {
  return axios.delete(API_URL + id, { headers: authHeader() });
};

// Funkciju za update ćemo dodati kasnije
// const updateCategory = (id, name) => { ... };

const categoryService = {
  getCategories,
  createCategory,
  deleteCategory,
};

export default categoryService;