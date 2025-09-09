import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import quizService from '../../services/quizService'; 
import QuizCard from '../../components/QuizCard/QuizCard';
import './QuizListPage.css';

interface QuizCardData {
  quizID: number;
  name: string;
  description: string;
  difficulty: string;
  maxPoints: number;
  categories: string[];
}


const QuizListPage: React.FC = () => {
  const [quizzes, setQuizzes] = useState<QuizCardData[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate(); 

  useEffect(() => {
    const activeQuizId = localStorage.getItem('activeQuizId');
    if (activeQuizId) {
        navigate(`/quiz/${activeQuizId}`);
        return; 
    }

    const fetchQuizzes = async () => {
      try {
        const response = await quizService.getQuizzes();
        setQuizzes(response.data);
      } catch (err) {
        setError('Došlo je do greške prilikom preuzimanja kvizova.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchQuizzes();
  }, [navigate]);

  if (loading) {
    return <div className="quiz-list-container"><h2>Učitavanje kvizova...</h2></div>;
  }

  if (error) {
    return <div className="quiz-list-container"><h2 className="error-message">{error}</h2></div>;
  }

  return (
    <div className="quiz-list-container">
      <h1>Dostupni Kvizovi</h1>
      <p>Izaberite kviz i testirajte svoje znanje!</p>
      <div className="quiz-grid">
        {quizzes.map(quiz => (
          <QuizCard key={quiz.quizID} quiz={quiz} />
        ))}
      </div>
    </div>
  );
};

export default QuizListPage;