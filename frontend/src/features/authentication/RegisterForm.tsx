import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../../services/authService';
import './Auth.css';

interface RegisterFormProps {
  onSuccess: () => void; // Funkcija koja će se pozvati nakon uspešne registracije
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onSuccess }) => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const navigate = useNavigate();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage('');

    try {
      await authService.register(username, email, password);
      onSuccess(); 
      navigate('/kvizovi');  
    } catch (error: any) {
      const resMessage = error.response?.data || 'Došlo je do greške!';
      setMessage(resMessage);
    }
  };

  return (
    <div className="auth-container-modal">
      <form onSubmit={handleRegister} className="auth-form">
        <h2>Registracija</h2>
        
        <div className="form-group">
          <label htmlFor="username">Korisničko ime</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="password">Lozinka</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={6}
          />
        </div>

        <div className="form-footer">
            <div className="form-message">
                {message && (
                    <span className="alert-danger">{message}</span>
                )}
            </div>
            <button type="submit" className="btn">Registruj se</button>
        </div>

      </form>
    </div>
  );
};

export default RegisterForm;