import React from 'react';
import { useNavigate } from 'react-router-dom';
import './QuizCard.css';

interface QuizCardProps {
  quiz: {
    quizID: number;
    name: string;
    description: string;
    difficulty: string;
    maxPoints: number;
    categories: string[];
  };
}

const QuizCard: React.FC<QuizCardProps> = ({ quiz }) => {
  const navigate = useNavigate();

  const handleStartQuiz = () => {
    navigate(`/quiz/${quiz.quizID}`);
  };

  return (
    <div className={`quiz-card difficulty-${quiz.difficulty.toLowerCase()}`}>
      
      <div className="quiz-card-header">
        <h3>{quiz.name}</h3>
        <span className="quiz-difficulty">{quiz.difficulty}</span>
      </div>

      <div className="quiz-card-categories">
        {quiz.categories.map((category, index) => (
          <span key={index} className="category-tag">{category}</span>
        ))}
      </div>
      
      <div className="quiz-card-body">
        <p>{quiz.description}</p>
      </div>
      
      <div className="quiz-card-footer">
        <span className="quiz-points">
          Max bodova: <strong>{quiz.maxPoints.toLocaleString('sr-RS')}</strong>
        </span>
        <button onClick={handleStartQuiz} className="btn-start-quiz">
          Započni Kviz 🚀
        </button>
      </div>
    </div>
  );
};

export default QuizCard;