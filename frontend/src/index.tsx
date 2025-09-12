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
    const originalRequest = error.config;
    const isAuthError = error.response && error.response.status === 401;
    if (isAuthError && originalRequest.url === `${process.env.REACT_APP_API_URL}/Auth/login`) {
        return Promise.reject(error);
    }

    if (isAuthError) {
        console.error("Token istekao ili je nevalidan. Odjavljivanje...");
        localStorage.removeItem('user_token');
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

