import React from 'react';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import HomePage from './features/home/HomePage';
import Navbar from './components/Navbar/Navbar'; 
import Footer from './components/Footer/Footer'; 
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import QuizListPage from './features/quiz/QuizListPage'; 

function App() {
  return (
    <div className="App">
      <Navbar />
      <main className="main-content">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route 
            path="/kvizovi" 
            element={
              <ProtectedRoute>
                <QuizListPage />
              </ProtectedRoute>
            } 
          />
        </Routes>
      </main>
      <Footer />
    </div>
  );
}

export default App;
