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

const updateCategory = (id: number, data: { name: string }) => {
  return axios.put(`${API_URL}${id}`, data, { headers: authHeader() });
};

const categoryService = {
  getCategories,
  createCategory,
  deleteCategory,
  updateCategory,
};

export default categoryService;