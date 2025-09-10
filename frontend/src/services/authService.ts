import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL;

const register = (username: string, email: string, password: string, profilePicture: File) => {
  const formData = new FormData();

  formData.append('username', username);
  formData.append('email', email);
  formData.append('password', password);
  formData.append('profilePicture', profilePicture);
  
  return axios.post(`${API_BASE_URL}/Auth/register`, formData);
};

const login = (username: string, password: string) => {
  return axios.post(`${API_BASE_URL}/Auth/login`, {
    username,
    password,
  });
};


const authService = {
  register,
  login
};

export default authService;