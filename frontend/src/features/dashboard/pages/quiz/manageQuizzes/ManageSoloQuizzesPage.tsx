import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Quiz } from '../types';
import QuizTable from '../components/quizTable/QuizTable';
import quizService from '../../../../../services/quizService';
import './style.css';

const ManageSoloQuizzesPage = () => {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    quizService.getSoloQuizzes().then((response) => {
      setQuizzes(response.data);
    });
  }, []);

  const handleEdit = async (id: number) => {
    navigate(`/dashboard/kvizovi/edit/${id}`);
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Da li ste sigurni?')) {
      await quizService.deleteQuiz(id);
      setQuizzes(quizzes.filter(q => q.quizID !== id));
    }
  };

  return (
    <div className="manage-page-container">
      <div className="manage-header">
        <h2>Upravljanje Kvizovima</h2>
        <button onClick={() => navigate('/dashboard/kvizovi/novi')} className="btn btn-primary">
          Dodaj Novi Kviz
        </button>
      </div>

       <QuizTable
                quizzes={quizzes}
                mode="solo"
                onEdit={handleEdit}
                onDelete={handleDelete}
            />
    </div>
  );
};

export default ManageSoloQuizzesPage;