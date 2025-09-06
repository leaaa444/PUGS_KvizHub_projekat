import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../../services/authService';
import './Auth.css'; 

interface LoginFormProps {
  onSuccess: () => void; 
}

const LoginForm: React.FC<LoginFormProps> = ({ onSuccess }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage('');

    try {
      const response = await authService.login(username, password);
      if (response.data.token) {
        localStorage.setItem('user_token', response.data.token);
        onSuccess(); 
        navigate('/kvizovi');       }
    } catch (error) {
      setMessage('Neuspešna prijava. Proverite korisničko ime i lozinku.');
    }
  };

  return (
    <div className="auth-container-modal">
      <form onSubmit={handleLogin} className="auth-form">
        <h2>Prijava</h2>
        <div className="form-group">
          <label htmlFor="login-username">Korisničko ime</label>
          <input
            type="text"
            id="login-username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="login-password">Lozinka</label>
          <input
            type="password"
            id="login-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <div className="form-footer">
          <div className="form-message">
            {message && (
              <span className="alert-danger">{message}</span>
            )}
          </div>
          <button type="submit" className="btn">Prijavi se</button>
        </div>
      </form>
    </div>
  );
};

export default LoginForm;