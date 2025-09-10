import React, {useState} from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const [isDropdownOpen, setDropdownOpen] = useState(false);

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
            Moj Nalog ▼ 
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