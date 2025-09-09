import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import quizService from '../../../../../services/quizService';
import './style.css';

interface Quiz {
  quizID: number;
  name: string;
  description: string;
  difficulty: string;
  timesCompleted: number;
  maxPoints: number;
  numberOfQuestions: number; 
}

const ManageQuizzesPage = () => {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    quizService.getQuizzes().then((response) => {
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

      <table className="manage-table">
        <thead>
          <tr>
            <th>Naziv</th>
            <th>Opis</th>
            <th>Težina</th>
            <th>Max bodova</th>
            <th>Broj pitanja</th>
            <th>Broj rešavanja</th>
            <th>Akcije</th>
          </tr>
        </thead>
        <tbody>
          {quizzes.map((quiz) => (
            <tr key={quiz.quizID}>
              <td>{quiz.name}</td>
              <td>{quiz.description}</td>
              <td>{quiz.difficulty}</td>
              <td>{quiz.maxPoints.toLocaleString('sr-RS')}</td>
              <td>{quiz.numberOfQuestions}</td>
              <td>{quiz.timesCompleted}</td>
              <td className="actions-cell">
                <button onClick={() => handleEdit(quiz.quizID)} className="btn-edit">Izmeni</button>
                <button onClick={() => handleDelete(quiz.quizID)} className="btn-delete">Obriši</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ManageQuizzesPage;