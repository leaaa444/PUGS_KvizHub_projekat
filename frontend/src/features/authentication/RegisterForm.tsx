import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Auth.css';

interface RegisterFormProps {
  onSuccess: () => void; 
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onSuccess }) => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const [profilePicture, setProfilePicture] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const { register } = useAuth();
  const navigate = useNavigate();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (file) {
          setProfilePicture(file);
          setImagePreview(URL.createObjectURL(file));
      }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage('');

    if (!profilePicture) {
        setMessage('Molimo izaberite profilnu sliku.');
        return;
    }

    try {
      await register(username, email, password, profilePicture);

      onSuccess(); 
      navigate('/kvizovi');

    } catch (error: any) {
      const resMessage = error.response?.data?.message || 'Došlo je do greške!';
      setMessage(resMessage);
    }
  };

  return (
    <div className="auth-container-modal">
      <form onSubmit={handleRegister} className="auth-form">
        <h2>Registracija</h2>
        
        <div className="auth-form-group">
          <label htmlFor="username">Korisničko ime</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>

        <div className="auth-form-group">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>

        <div className="auth-form-group">
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

        <div className="auth-form-group">
          <label htmlFor="profilePicture">Profilna slika</label>
          <input
            type="file"
            id="profilePicture"
            onChange={handleFileChange}
            accept="image/png, image/jpeg" 
            required
          />
        </div>

        {imagePreview && (
            <div className="image-preview-container">
                <img src={imagePreview} alt="Pregled profilne slike" className="profile-image-preview" />
            </div>
        )}

        <div className="auth-form-footer">
            <div className="auth-form-message">
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