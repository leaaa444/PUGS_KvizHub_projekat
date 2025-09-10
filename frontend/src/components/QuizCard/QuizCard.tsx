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
    numberOfQuestions: number;
    timeLimit: number;
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

      <div className="quiz-card-meta">
        <span className="meta-item">
          <svg /* Ikona za pitanja */ xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M6 3.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5M9 6.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5m-3 3a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5m2 3a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5m-5-6a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 0 1h-1a.5.5 0 0 1-.5-.5m0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 0 1h-1a.5.5 0 0 1-.5-.5m0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 0 1h-1a.5.5 0 0 1-.5-.5"/>
            <path d="M2 1a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1zM1 2a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2z"/>
          </svg>
          {quiz.numberOfQuestions} pitanja
        </span>
        <span className="meta-item">
          <svg /* Ikona za sat */ xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8 3.5a.5.5 0 0 0-1 0V9a.5.5 0 0 0 .252.434l3.5 2a.5.5 0 0 0 .496-.868L8 8.71z"/>
            <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16m7-8A7 7 0 1 1 1 8a7 7 0 0 1 14 0"/>
          </svg>
          {quiz.timeLimit} sec
        </span>
      </div>
      
      <div className="quiz-card-footer">
        <span className="quiz-points">
          Max bodova: <strong>{quiz.maxPoints.toLocaleString('sr-RS')}</strong>
        </span>
        <button onClick={handleStartQuiz} className="btn-start-quiz">
          Započni Kviz!
        </button>
      </div>
    </div>
  );
};

export default QuizCard;