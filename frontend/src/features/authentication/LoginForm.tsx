import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Auth.css'; 

interface LoginFormProps {
  onSuccess: () => void; 
}

const LoginForm: React.FC<LoginFormProps> = ({ onSuccess }) => {
  const [identifier, setIdentifier] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const { login } = useAuth();

  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage('');
    try {
      await login(identifier, password); 
      onSuccess(); 
      navigate('/kvizovi');
    } catch (error) {
      setMessage('Neuspešna prijava. Proverite korisničko ime i lozinku.');
    }
  };

  return (
    <div className="auth-container-modal">
      <form onSubmit={handleLogin} className="auth-form">
        <h2>Prijava</h2>
        <div className="auth-form-group">
          <label htmlFor="login-username">Korisničko ime ili Email</label>
          <input
            type="text"
            id="login-username"
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
            required
          />
        </div>
        <div className="auth-form-group">
          <label htmlFor="login-password">Lozinka</label>
          <input
            type="password"
            id="login-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <div className="auth-form-footer">
          <div className="auth-form-message">
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