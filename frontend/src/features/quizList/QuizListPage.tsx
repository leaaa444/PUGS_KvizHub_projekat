import React, { useState, useEffect, useMemo } from 'react';
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
  numberOfQuestions: number;
  timeLimit: number;
  
}


const QuizListPage: React.FC = () => {
  const [allQuizzes, setAllQuizzes] = useState<QuizCardData[]>([]); 
  const [filteredQuizzes, setFilteredQuizzes] = useState<QuizCardData[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [selectedDifficulty, setSelectedDifficulty] = useState<string>('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');

  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate(); 

  useEffect(() => {
    const activeQuizStateJSON = localStorage.getItem('activeQuizState');

    if (activeQuizStateJSON) {
      try {
        const activeQuizState = JSON.parse(activeQuizStateJSON);
          if (activeQuizState && activeQuizState.quizId) {
            navigate(`/quiz/${activeQuizState.quizId}`);
            return;
          }
      }catch (e) {
        localStorage.removeItem('activeQuizState');
      }
    }

    const fetchQuizzes = async () => {
      try {
        const response = await quizService.getQuizzes();
        setAllQuizzes(response.data);
        setFilteredQuizzes(response.data);
      } catch (err) {
        setError('Došlo je do greške prilikom preuzimanja kvizova.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchQuizzes();
  }, [navigate]);


  useEffect(() => {
    let quizzesToFilter = [...allQuizzes];

    if (searchTerm) {
      quizzesToFilter = quizzesToFilter.filter(quiz =>
        quiz.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        quiz.description.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    if (selectedDifficulty) {
      quizzesToFilter = quizzesToFilter.filter(quiz => quiz.difficulty === selectedDifficulty);
    }

    if (selectedCategory) {
      quizzesToFilter = quizzesToFilter.filter(quiz => quiz.categories.includes(selectedCategory));
    }

    setFilteredQuizzes(quizzesToFilter);
  }, [searchTerm, selectedDifficulty, selectedCategory, allQuizzes]);

 
  const uniqueCategories = useMemo(() => {
    const allCategories = allQuizzes.flatMap(quiz => quiz.categories);
    return [...new Set(allCategories)]; 
  }, [allQuizzes]);

  const uniqueDifficulties = useMemo(() => {
    const allDifficulties = allQuizzes.map(quiz => quiz.difficulty);
    return [...new Set(allDifficulties)]; 
  }, [allQuizzes]);

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
      <div className="filter-container">
        <input
          type="text"
          placeholder="Pretraži po nazivu ili kljucnoj reci..."
          className="filter-search"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        <select
          className="filter-select"
          value={selectedDifficulty}
          onChange={(e) => setSelectedDifficulty(e.target.value)}
        >
          <option value="">Sve težine</option>
          {uniqueDifficulties.map(difficulty => (
              <option key={difficulty} value={difficulty}>{difficulty}</option>
          ))}
        </select>
        <select
          className="filter-select"
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
        >
          <option value="">Sve kategorije</option>
          {uniqueCategories.map(category => (
            <option key={category} value={category}>{category}</option>
          ))}
        </select>
      </div>

      <div className="quiz-grid">
        {filteredQuizzes.length > 0 ? (
          filteredQuizzes.map(quiz => (
            <QuizCard key={quiz.quizID} quiz={quiz} />
          ))
        ) : (
          <p className="no-quizzes-message">Nema kvizova koji odgovaraju zadatim kriterijumima.</p>
        )}
      </div>
    </div>
  );
};

export default QuizListPage;