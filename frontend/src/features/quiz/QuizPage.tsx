import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate, useBlocker } from 'react-router-dom';
import resultService from '../../services/resultService';
import { useQuizState } from './useQuizState';
import './QuizPage.css';


import Modal from '../../components/Modal/Modal';
import QuizHeader from './components/QuizHeader';
import QuestionArea from './components/QuestionArea';
import QuizNavigation from './components/QuizNavigation';
import { ResultData } from './types';

const QuizPage: React.FC = () => {
    const navigate = useNavigate();

    const {
        quizData, loading, timeLeft, userAnswers,
        currentQuestionIndex, setCurrentQuestionIndex, handleAnswerChange
    } = useQuizState();
    
    const [resultData, setResultData] = useState<ResultData | null>(null);

    const [isResultModalOpen, setIsResultModalOpen] = useState(false);
    const [showExitConfirm, setShowExitConfirm] = useState(false);

    const blocker = useBlocker(() => !isResultModalOpen && timeLeft > 0);
    
    const handleSubmit = useCallback(async (options = { showResultsModal: true }) => {
        if (!quizData) return;
        
        const submissionDto = {
            quizId: quizData.quizId,
            timeTaken: quizData.timeLimit - timeLeft,
            answers: Object.entries(userAnswers).map(([questionId, answer]) => ({
                questionId: parseInt(questionId, 10),
                answerOptionIds: Array.isArray(answer) ? answer : (typeof answer === 'number' ? [answer] : []),
                answerText: typeof answer === 'string' ? answer : null,
            })),
        };

         try {
            const response = await resultService.submitQuiz(submissionDto);
            localStorage.removeItem('activeQuizState');

            const resultDataResponse = {
                ...response.data,
                totalQuestions: quizData.questions.length
            };
            
            setResultData(resultDataResponse);

            sessionStorage.setItem('lastQuizResultId', resultDataResponse.resultId.toString());

            if (options.showResultsModal) {
            setIsResultModalOpen(true);
        }
        } catch (error) {
            console.error("Greška pri predaji kviza:", error);
            alert("Došlo je do greške prilikom predaje kviza.");
        }
    }, [quizData, timeLeft, userAnswers]);

    // Auto-submit on timer end
   useEffect(() => {
        if (timeLeft === 0 && !loading && quizData) {
            handleSubmit({ showResultsModal: true });
        }
    }, [timeLeft, loading, quizData, handleSubmit]);

    // Blocker logic
    useEffect(() => {
        if (blocker.state === 'blocked') {
            setShowExitConfirm(true);
        }
    }, [blocker]);

   const handleConfirmExit = async () => {
        setShowExitConfirm(false);
        
        blocker.reset?.();
        
        await handleSubmit({ showResultsModal: true });
    };
    const handleCancelExit = () => {
        // Poništi izlazak i zatvori modal
        setShowExitConfirm(false);
        blocker.reset?.(); // Resetuj blokiranje
    };
    
    
    if (loading || !quizData) {
        return <div className="quiz-page-container"><h2>Učitavanje kviza...</h2></div>;
    }

    if (quizData.questions.length === 0) {
        return <div className="quiz-page-container"><h2>Kviz nema pitanja.</h2></div>;
    }

    const handleReview = () => {
        if (resultData) {
            navigate(`/rezultati/${resultData.resultId}`, { replace: true });
        }
    };

    const currentQuestion = quizData.questions[currentQuestionIndex];

    return (
        <>
            <div className="quiz-page-container">
                <QuizHeader name={quizData.name} timeLeft={timeLeft} />

                <QuestionArea
                    currentQuestion={currentQuestion}
                    currentQuestionIndex={currentQuestionIndex}
                    totalQuestions={quizData.questions.length}
                    currentAnswer={userAnswers[currentQuestion.questionId]}
                    onAnswerChange={handleAnswerChange}
                    showOptions = {true}
                />

                <QuizNavigation
                    currentQuestionIndex={currentQuestionIndex}
                    totalQuestions={quizData.questions.length}
                    onPrev={() => setCurrentQuestionIndex(prev => prev - 1)}
                    onNext={() => setCurrentQuestionIndex(prev => prev + 1)}
                    onSubmit={() => handleSubmit({ showResultsModal: true })}
                />
            </div>

            <Modal isOpen={isResultModalOpen} onClose={() => navigate('/kvizovi', { replace: true })}>
                {resultData && (
                    <div className="quiz-result-content">
                        <h2>Kviz Predat!</h2>
                        <div className="result-summary">
                            <div className="result-item">
                                <span className="result-label">Osvojeni Bodovi</span>
                                <span className="result-value score">
                                    {resultData.score.toLocaleString('sr-RS')} / {resultData.maxPossibleScore.toLocaleString('sr-RS')}
                                </span>
                            </div>
                            <div className="result-item">
                                <span className="result-label">Tačni Odgovori</span>
                                <span className="result-value">
                                    {resultData.correctAnswers} / {resultData.totalQuestions}
                                </span>
                            </div>
                        </div>
                        <div className="modal-actions">
                            <button onClick={handleReview} className="btn-review">
                                Pregledaj Odgovore
                            </button>
                        </div>
                    </div>
                )}
            </Modal>

             <Modal isOpen={showExitConfirm} onClose={handleCancelExit}>
                <div className="exit-confirm-content">
                    <h3>Da li ste sigurni?</h3>
                    <p>Ako napustite stranicu, vaš kviz će biti automatski predat sa trenutnim odgovorima.</p>
                    <div className="modal-actions">
                        <button type="button" onClick={handleConfirmExit} className="btn-confirm-exit">
                            Da, predaj i izađi
                        </button>
                        <button onClick={handleCancelExit} className="btn-cancel-exit">
                            Ne, ostajem
                        </button>
                    </div>
                </div>
            </Modal>
        </>
    );
};

export default QuizPage;