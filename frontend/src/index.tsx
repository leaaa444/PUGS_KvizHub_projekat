import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext'; 
import axios from 'axios';

axios.interceptors.response.use(
  (response) => response,
  
  (error) => {
    if (error.response && error.response.status === 401) {
      // Token je nevalidan ili istekao
      console.error("Auth Error 401: Token je nevalidan ili istekao. Odjavljivanje...");
      
      // Brišemo token iz memorije
      localStorage.removeItem('user_token');
      
      // Forsiramo reload aplikacije i preusmeravamo na početnu stranicu
      window.location.href = '/'; 
    }
    
    return Promise.reject(error);
  }
);

const router = createBrowserRouter([
  {
    path: "*", 
    Component: App, 
  },
]);

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <AuthProvider> 
      <RouterProvider router={router} />
    </AuthProvider>
  </React.StrictMode>
);

