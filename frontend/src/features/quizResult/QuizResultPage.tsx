import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import resultService from '../../services/resultService';
import QuestionReview from './QuestionReview';
import { QuizResultDetails } from './types';
import './QuizResultPage.css';

const QuizResultPage: React.FC = () => {
    const { resultId } = useParams<{ resultId: string }>();
    const navigate = useNavigate();
    const [resultDetails, setResultDetails] = useState<QuizResultDetails | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const lastResultId = sessionStorage.getItem('lastQuizResultId');
        if (!resultId) return;

        if (!resultId || lastResultId !== resultId) {
            console.warn("Pokušaj pristupa nevalidnoj stranici rezultata.");
            navigate('/kvizovi', { replace: true }); 
            return; 
        }

        const fetchResultDetails = async () => {
            try {
                const response = await resultService.getResultDetails(parseInt(resultId, 10));
                setResultDetails(response.data);
            } catch (error) {
                console.error("Greška pri preuzimanju detalja rezultata:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchResultDetails();

    }, [resultId, navigate]);

    const handleGoBack = () => {
        sessionStorage.removeItem('lastQuizResultId');
        navigate('/kvizovi');
    };

    if (loading) {
        return <div className="quiz-result-container"><h2>Učitavanje rezultata...</h2></div>;
    }

    if (!resultDetails) {
        return <div className="quiz-result-container"><h2>Rezultat nije pronađen.</h2></div>;
    }

    const { quizName, score, maxPossibleScore, questions, dateCompleted, timeTaken } = resultDetails;
    const percentage = ((score / maxPossibleScore) * 100).toFixed(1);

    return (
        <div className="quiz-result-container">
            <h1>Pregled Kviza: {quizName}</h1>
            <div className="result-summary-header">
                <div className="summary-item"><strong>Datum:</strong> {new Date(dateCompleted).toLocaleDateString('sr-RS')}</div>
                <div className="summary-item"><strong>Bodovi:</strong> {score.toLocaleString('sr-RS')} / {maxPossibleScore.toLocaleString('sr-RS')} ({percentage}%)</div>
                <div className="summary-item"><strong>Vreme:</strong> {Math.floor(timeTaken / 60)} min {timeTaken % 60} sec</div>
            </div>

            <div className="questions-review-list">
                {questions.map((q, index) => (
                    <QuestionReview key={q.questionId} question={q} index={index + 1} />
                ))}
            </div>

            <div className="back-link-container">
                <button onClick={handleGoBack} className="btn-back">Nazad na kvizove</button>
            </div>
        </div>
    );
};

export default QuizResultPage;