import React from 'react';
import './Footer.css';

const Footer = () => {
  return (
    <footer className="footer">
      <p>&copy; {new Date().getFullYear()} KvizHub. Sva prava zadržana.</p>
    </footer>
  );
};

export default Footer;