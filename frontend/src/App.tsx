import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import './App.css';
import HomePage from './features/home/HomePage';
import Navbar from './components/Navbar/Navbar'; 
import Footer from './components/Footer/Footer'; 
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import PublicOnlyRoute from './components/ProtectedRoute/PublicOnlyRoute';
import AdminRoute from './components/ProtectedRoute/AdminRoute';
import QuizListPage from './features/quizList/QuizListPage'; 
import DashboardPage from './features/dashboard/pages/DashboardPage';
import DashboardLayout from './features/dashboard/DashboardLayout';
import ManageSoloQuizzesPage from './features/dashboard/pages/quiz/manageQuizzes/ManageSoloQuizzesPage';
import ManageCategoriesPage from './features/dashboard/pages/categories/ManageCategoriesPage';
import QuizBuilderPage from './features/dashboard/pages/quiz/quizbuilder/page';
import QuizPage from './features/quiz/QuizPage';
import QuizResultPage from './features/quizResult/QuizResultPage';
import MyResultsPage from './features/myResults/MyResultsPage';
import GlobalResultsPage from './features/globalResults/GlobalResultsPage';
import AdminResultsPage from './features/dashboard/pages/results/AdminResults';
import AccountSettingsPage from './features/account/AccountSettingsPage';

import ManageLiveQuizzesPage from './features/dashboard/pages/quiz/manageQuizzes/ManageLiveQuizzesPage';
import JoinArenaModal from './components/JoinArenaModal/JoinArenaModal';
import LobbyPage from './features/live-quiz/lobby/LobbyPage';
import LiveArena from './features/live-quiz/game/LiveArena';

function App() {
  const [isJoinModalOpen, setIsJoinModalOpen] = useState(false);

  
  return (
    <div className="App">
      <Navbar onOpenJoinModal={() => setIsJoinModalOpen(true)} />
      <main className="main-content">
        <Routes>
          <Route path="/" element={<PublicOnlyRoute><HomePage /></PublicOnlyRoute>} />
          <Route path="/kvizovi" element={<ProtectedRoute><QuizListPage /></ProtectedRoute>} />
          <Route path="/quiz/:quizId" element={<ProtectedRoute><QuizPage /></ProtectedRoute>} /> 
          <Route path="/rezultati/:resultId" element={<ProtectedRoute><QuizResultPage /></ProtectedRoute>} /> 
          <Route path="/moji-rezultati" element={<ProtectedRoute><MyResultsPage /></ProtectedRoute>} />
          <Route path="/rang-lista" element={<ProtectedRoute><GlobalResultsPage /></ProtectedRoute>} />
          <Route path="/nalog" element={<ProtectedRoute><AccountSettingsPage /></ProtectedRoute>} />

          <Route path="/live-arena/lobby/:roomCode" element={<ProtectedRoute><LobbyPage /></ProtectedRoute>} />
          <Route path="/live-arena/game/:roomCode" element={<ProtectedRoute><LiveArena /></ProtectedRoute>} />

          <Route path="/dashboard" element={<AdminRoute><DashboardLayout /></AdminRoute>}>
            <Route index element={<DashboardPage />} />
            {/* --- Solo Kvizovi --- */}
            <Route path="kvizovi" element={<ManageSoloQuizzesPage />} />
            <Route path="kvizovi/novi" element={<QuizBuilderPage mode="solo" />} />

            {/* --- Live Kvizovi --- */}
            <Route path="live-kvizovi" element={<ManageLiveQuizzesPage />} />
            <Route path="live-kvizovi/novi" element={<QuizBuilderPage mode="live" />} />
            
            {/* --- Zajednička ruta za izmenu --- */}
            <Route path="kvizovi/edit/:quizId" element={<QuizBuilderPage />}/>
            <Route path="kategorije" element={<ManageCategoriesPage />} />
            <Route path="rezultati" element={<AdminResultsPage />} />
          </Route>

        </Routes>
      </main>

      <JoinArenaModal 
        isOpen={isJoinModalOpen} 
        onClose={() => setIsJoinModalOpen(false)} 
      />

      <Footer />
    </div>
  );
}

export default App;
