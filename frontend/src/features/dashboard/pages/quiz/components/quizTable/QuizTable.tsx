import React from 'react';
import { Quiz } from '../../types';

interface QuizTableProps {
    quizzes: Quiz[];
    mode: 'solo' | 'live';
    onEdit: (id: number) => void;
    onDelete: (id: number) => void;
    onStartArena?: (id: number) => void; 
}

const QuizTable: React.FC<QuizTableProps> = ({ quizzes, mode, onEdit, onDelete, onStartArena }) => {
    return (
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
                            <div className="actions-cell-wrapper">
                                {mode === 'live' && onStartArena && (
                                    <button onClick={() => onStartArena(quiz.quizID)} className="btn-start-arena">
                                        Pokreni Arenu
                                    </button>
                                )}
                                <button onClick={() => onEdit(quiz.quizID)} className="btn-edit">Izmeni</button>
                                <button onClick={() => onDelete(quiz.quizID)} className="btn-delete">Obriši</button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default QuizTable;