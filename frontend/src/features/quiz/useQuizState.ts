// src/hooks/useQuizState.ts
import { useState, useEffect, useCallback, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import quizService from '../../services/quizService';
import { QuizData, UserAnswers } from './types';

// Stanje koje čuvamo u localStorage
interface ActiveQuizState  {
    quizId: number;
    userAnswers: UserAnswers;
    endTime: number;
    currentQuestionIndex: number;
}

// Nova funkcija koja čita stanje iz jednog ključa
const getInitialState = (quizId: number): ActiveQuizState  | null => {
    const savedStateJSON = localStorage.getItem('activeQuizState');
    if (!savedStateJSON) return null;

    try {
        const savedState: ActiveQuizState  = JSON.parse(savedStateJSON);
        if (savedState.quizId === quizId) {
            if (savedState.endTime > Date.now()) {
                return savedState;
            }
        }
        // Ako je sačuvan neki drugi kviz, brišemo ga
        localStorage.removeItem('activeQuizState');
        return null;
    } catch (e) {
        localStorage.removeItem('activeQuizState');
        return null;
    }
};

export const useQuizState = () => {
    const { quizId: quizIdParam } = useParams<{ quizId: string }>();
    const quizId = Number(quizIdParam);
    const navigate = useNavigate();

    const [quizData, setQuizData] = useState<QuizData | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    const initialState = useMemo(() => getInitialState(quizId), [quizId]);
    
    const [userAnswers, setUserAnswers] = useState<UserAnswers>(initialState?.userAnswers || {});
    const [currentQuestionIndex, setCurrentQuestionIndex] = useState<number>(initialState?.currentQuestionIndex || 0);
    const [timeLeft, setTimeLeft] = useState<number>(initialState ? Math.round((initialState.endTime - Date.now()) / 1000) : 0);
    
    // za inicijalizaciju kviza
    useEffect(() => {
        if (!quizId) return;

        quizService.getQuizForTaker(quizId)
            .then(response => {
                const fetchedQuizData = response.data;
                setQuizData(fetchedQuizData);

                if (!initialState) {
                    const endTime = Date.now() + fetchedQuizData.timeLimit * 1000;
                    setTimeLeft(fetchedQuizData.timeLimit);
                    
                    // Kreiramo i čuvamo ceo 'activeQuizState' objekat odjednom
                    const newState: ActiveQuizState = {
                        quizId: quizId,
                        userAnswers: {},
                        currentQuestionIndex: 0,
                        endTime: endTime
                    };
                    localStorage.setItem('activeQuizState', JSON.stringify(newState));
                }
                setLoading(false);

            })
            .catch(error => {
                console.error("Greška pri preuzimanju kviza:", error);
                navigate('/kvizovi');
            });
    }, [quizId, navigate, initialState]);

    // za čuvanje kompletnog napretka
     useEffect(() => {
        if (loading) return;

        const currentStateJSON = localStorage.getItem('activeQuizState');
        if (currentStateJSON) {
            try {
                const currentState: ActiveQuizState = JSON.parse(currentStateJSON);
                const newState: ActiveQuizState = {
                    ...currentState,
                    userAnswers,
                    currentQuestionIndex
                };
                localStorage.setItem('activeQuizState', JSON.stringify(newState));
            } catch (e) {
                console.error("Greška pri čuvanju stanja kviza:", e);
            }
        }
    }, [userAnswers, currentQuestionIndex, loading]);

    // Timer logic
    useEffect(() => {
        if (timeLeft <= 0 || loading) return;
        const timerId = setInterval(() => setTimeLeft(prev => prev - 1), 1000);
        return () => clearInterval(timerId);
    }, [timeLeft, loading]);

    const handleAnswerChange = useCallback((questionId: number, answerValue: any, questionType: string) => {
    setUserAnswers(prev => {
        const newAnswers = { ...prev };

        if (questionType === 'MultipleChoice') {
            const currentSelection = (newAnswers[questionId] as number[] | undefined) || [];
            
            if (currentSelection.includes(answerValue)) {
                newAnswers[questionId] = currentSelection.filter(id => id !== answerValue);
            } else {
                newAnswers[questionId] = [...currentSelection, answerValue];
            }
        } else {
            newAnswers[questionId] = answerValue;
        }

        return newAnswers;
    });
}, []);
    
    return {
        quizId,
        quizData,
        loading,
        timeLeft,
        userAnswers,
        currentQuestionIndex,
        setCurrentQuestionIndex,
        handleAnswerChange,
        setTimeLeft
    };
};