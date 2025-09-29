import React from 'react';
import { NavLink } from 'react-router-dom';
import './Sidebar.css';

const Sidebar = () => {
  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h3>Admin Panel</h3>
      </div>
      <nav className="sidebar-nav">
        <NavLink to="/dashboard" className="sidebar-link" end>
          <span>Dashboard</span>
        </NavLink>
        <NavLink to="/dashboard/kvizovi" className="sidebar-link">
          <span>Upravljaj kvizovima</span>
        </NavLink>
        <NavLink to="/dashboard/live-kvizovi" className="sidebar-link">
          <span>Upravljaj kvizovima uzivo</span>
        </NavLink>
        <NavLink to="/dashboard/kategorije" className="sidebar-link">
          <span>Upravljaj kategorijama</span>
        </NavLink>
        <NavLink to="/dashboard/rezultati" className="sidebar-link">
          <span>Pregled rezultata</span>
        </NavLink>
      </nav>
    </aside>
  );
};

export default Sidebar;