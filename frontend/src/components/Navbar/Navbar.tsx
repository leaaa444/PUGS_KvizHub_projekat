import React, {useState} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
  const navigate = useNavigate();

  const token = localStorage.getItem('user_token');

  const [isDropdownOpen, setDropdownOpen] = useState(false);

  if (!token) {
    return null; 
  }

  const handleLogout = () => {
    localStorage.removeItem('user_token'); 
    navigate('/'); 
    window.location.reload(); 
  };

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-logo">
        KvizHub
      </Link>

      <div className="navbar-links">
        <Link to="/kvizovi" className="navbar-links">Kvizovi</Link>
        <Link to="/rang-lista" className="navbar-links">Rang Lista</Link>
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