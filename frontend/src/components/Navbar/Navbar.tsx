import React, {useState} from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const [isDropdownOpen, setDropdownOpen] = useState(false);

const fullProfilePictureUrl = user && user.profilePictureUrl ? `${process.env.REACT_APP_API_BASE_URL}${user.profilePictureUrl}` : '';

  if (!user) { 
    return null;
  }

  const handleLogout = () => {
    setDropdownOpen(false);
    logout();
  };

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-logo">
        KvizHub
      </Link>

      <div className="navbar-links">
        <Link to="/kvizovi" className="navbar-links">Kvizovi</Link>
        <Link to="/moji-rezultati" className="navbar-links">Moji rezultati</Link>
        <Link to="/rang-lista" className="navbar-links">Rang Lista</Link>
        {user.role === 'Admin' && (
          <Link to="/dashboard" className="navbar-links">Dashboard</Link>
        )}
      </div>

      <div className="user-menu">
          <button 
            onClick={() => setDropdownOpen(!isDropdownOpen)} 
            className="user-menu-trigger"
          >
            <img 
              src={fullProfilePictureUrl} 
              alt="Profilna slika" 
              className="navbar-profile-pic" 
            /> 
            <span>▼</span>
          </button>

          {isDropdownOpen && (
            <div className="dropdown-menu">
              <Link to="/nalog" className="dropdown-item">Podešavanja Naloga</Link>
              <button onClick={handleLogout} className="dropdown-item logout-button">
                Odjavi se
              </button>
            </div>
          )}
      </div>
    </nav>
  );
};

export default Navbar;