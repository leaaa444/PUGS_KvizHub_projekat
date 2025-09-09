import React, { useState } from 'react';
import './HomePage.css'; 
import Modal from '../../components/Modal/Modal'; 
import RegisterForm from '../authentication/RegisterForm'; 
import LoginForm from '../authentication/LoginForm'; 

const HomePage = () => {

  const [isRegisterModalOpen, setRegisterModalOpen] = useState(false);
  const [isLoginModalOpen, setLoginModalOpen] = useState(false);

  return (
     <> 
      <div className="home-container">
        <h1>Dobrodošli na KvizHub!</h1>
        <p>Testirajte svoje znanje i takmičite se sa drugima.</p>
        <div>
          <button onClick={() => setLoginModalOpen(true)} className="btn home-buttons">
            Prijavi se
          </button>
          
          <button onClick={() => setRegisterModalOpen(true)} className="btn home-buttons">
            Registruj se
          </button>
        </div>
      </div>

      <Modal isOpen={isRegisterModalOpen} onClose={() => setRegisterModalOpen(false)}>
        <RegisterForm onSuccess={() => setRegisterModalOpen(false)} />
      </Modal>

      <Modal isOpen={isLoginModalOpen} onClose={() => setLoginModalOpen(false)}>
        <LoginForm onSuccess={() => setLoginModalOpen(false)} />
      </Modal>
    </>
  );
};

export default HomePage;