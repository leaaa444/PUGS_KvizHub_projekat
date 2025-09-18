import React from 'react';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import HomePage from './features/home/HomePage';
import Navbar from './components/Navbar/Navbar'; 
import Footer from './components/Footer/Footer'; 
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import PublicOnlyRoute from './components/ProtectedRoute/PublicOnlyRoute';
import AdminRoute from './components/ProtectedRoute/AdminRoute';
import QuizListPage from './features/quizList/QuizListPage'; 
import DashboardPage from './features/dashboard/pages/DashboardPage';
import DashboardLayout from './features/dashboard/DashboardLayout';
import ManageQuizzesPage from './features/dashboard/pages/quiz/manageQuizzes/page';
import ManageCategoriesPage from './features/dashboard/pages/categories/ManageCategoriesPage';
import QuizBuilderPage from './features/dashboard/pages/quiz/quizbuilder/page';
import QuizPage from './features/quiz/QuizPage';
import QuizResultPage from './features/quizResult/QuizResultPage';
import MyResultsPage from './features/myResults/MyResultsPage';
import GlobalResultsPage from './features/globalResults/GlobalResultsPage';
import AdminResultsPage from './features/dashboard/pages/results/AdminResults';
import AccountSettingsPage from './features/account/AccountSettingsPage';

function App() {
  return (
    <div className="App">
      <Navbar />
      <main className="main-content">
        <Routes>

          <Route path="/" element={<PublicOnlyRoute><HomePage /></PublicOnlyRoute>} />
          <Route path="/kvizovi" element={<ProtectedRoute><QuizListPage /></ProtectedRoute>} />
          <Route path="/quiz/:quizId" element={<ProtectedRoute><QuizPage /></ProtectedRoute>} /> 
          <Route path="/rezultati/:resultId" element={<ProtectedRoute><QuizResultPage /></ProtectedRoute>} /> 
          <Route path="/moji-rezultati" element={<ProtectedRoute><MyResultsPage /></ProtectedRoute>} />
          <Route path="/rang-lista" element={<ProtectedRoute><GlobalResultsPage /></ProtectedRoute>} />
          <Route path="/nalog" element={<ProtectedRoute><AccountSettingsPage /></ProtectedRoute>} />
          <Route path="/dashboard" element={<AdminRoute><DashboardLayout /></AdminRoute>}>
            <Route index element={<DashboardPage />} />
            <Route path="kvizovi" element={<ManageQuizzesPage />} />
            <Route path="kvizovi/novi" element={<QuizBuilderPage />} />      
            <Route path="kvizovi/edit/:quizId" element={<QuizBuilderPage />}/>
            <Route path="kategorije" element={<ManageCategoriesPage />} />
            <Route path="rezultati" element={<AdminResultsPage />} />
          </Route>

        </Routes>
      </main>
      <Footer />
    </div>
  );
}

export default App;
