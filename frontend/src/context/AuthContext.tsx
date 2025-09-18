import React, { createContext, useState, useContext, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import authService from '../services/authService';

interface User {
  username: string;
  role: string;
  profilePictureUrl: string; 
  email: string;
}

interface AuthContextType {
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  loading: boolean;
  register: (username: string, email: string, password: string, profilePicture: File) => Promise<void>;
  logout: () => void;
  token: string | null;
  setUser: React.Dispatch<React.SetStateAction<User | null>>;
}

const AuthContext = createContext<AuthContextType>(null!);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(localStorage.getItem('user_token'));
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    try {
      if (token) {
        const decodedUser: any = jwtDecode(token);
        if (decodedUser.exp * 1000 > Date.now()) {
          setUser({ 
            username: decodedUser.unique_name, 
            role: decodedUser.role ,
            profilePictureUrl: decodedUser.profilePictureUrl,
            email: decodedUser.email
          });
        } else {
          logout();
        }
      }
    } catch (e) {
      logout();
    } finally {
      setLoading(false); 
    }
  }, [token]);

    const register = async (username: string, email: string, password: string, profilePicture: File) => {
    await authService.register(username, email, password, profilePicture);
    await login(username, password); 
  };

  const login = async (username: string, password: string) => {
    try {
        const response = await authService.login(username, password);
        
        if (response.data.token) {
            const newToken = response.data.token;
            localStorage.setItem('user_token', newToken);
            const decodedUser: any = jwtDecode(newToken);
            setUser({ 
                username: decodedUser.unique_name, 
                role: decodedUser.role,
                profilePictureUrl: decodedUser.profilePictureUrl,
                email: decodedUser.email
            });
            setToken(newToken);
        }
    } catch (error) {
        console.error("Greška pri prijavi (AuthContext):", error);
        throw error; 
    }
  };

  const logout = () => {
    localStorage.removeItem('user_token');
    setUser(null);
    setToken(null);
  };

  const value = { user, loading, login, register,  logout, token, setUser  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  return useContext(AuthContext);
};