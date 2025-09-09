// src/hooks/useQuizState.ts
import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import quizService from '../../services/quizService';
import { QuizData } from './types';

// Funkcija za inicijalizaciju stanja iz localStorage-a
const getInitialState = <T>(key: string, defaultValue: T): T => {
    const savedValue = localStorage.getItem(key);
    if (savedValue) {
        try {
            return JSON.parse(savedValue);
        } catch (e) {
            return savedValue as T;
        }
    }
    return defaultValue;
};

export const useQuizState = () => {
    const { quizId } = useParams<{ quizId: string }>();
    const navigate = useNavigate();

    const [quizData, setQuizData] = useState<QuizData | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [timeLeft, setTimeLeft] = useState<number>(0);
    
    const [userAnswers, setUserAnswers] = useState<{ [key: number]: any }>(() => 
        getInitialState(`quizAnswers_${quizId}`, {})
    );
    const [currentQuestionIndex, setCurrentQuestionIndex] = useState<number>(() => 
        parseInt(getInitialState(`quizLastIndex_${quizId}`, '0'), 10)
    );

    useEffect(() => {
        if (!quizId) return;
        const timerKey = `quizEndTime_${quizId}`;
        const savedEndTime = localStorage.getItem(timerKey);

        quizService.getQuizForTaker(parseInt(quizId))
            .then(response => {
                setQuizData(response.data);
                if (savedEndTime) {
                    const endTime = parseInt(savedEndTime, 10);
                    const remaining = Math.max(0, Math.round((endTime - Date.now()) / 1000));
                    setTimeLeft(remaining);
                } else {
                    const timeLimit = response.data.timeLimit;
                    const endTime = Date.now() + timeLimit * 1000;
                    localStorage.setItem(timerKey, endTime.toString());
                    setTimeLeft(timeLimit);
                }
                localStorage.setItem('activeQuizId', quizId);
                setLoading(false);
            })
            .catch(error => {
                console.error("Error fetching quiz:", error);
                navigate('/kvizovi');
            });
    }, [quizId, navigate]);

    // Efekti za čuvanje stanja u localStorage
    useEffect(() => {
        if (quizId) localStorage.setItem(`quizAnswers_${quizId}`, JSON.stringify(userAnswers));
    }, [userAnswers, quizId]);

    useEffect(() => {
        if (quizId) localStorage.setItem(`quizLastIndex_${quizId}`, currentQuestionIndex.toString());
    }, [currentQuestionIndex, quizId]);

    // Timer logic
    useEffect(() => {
        if (timeLeft <= 0 || loading) return;
        const timerId = setInterval(() => setTimeLeft(prev => prev - 1), 1000);
        return () => clearInterval(timerId);
    }, [timeLeft, loading]);

    const handleAnswerChange = useCallback((questionId: number, answerValue: any, questionType: number) => {
        setUserAnswers(prev => {
            const newAnswers = { ...prev };
            if (questionType === 1) { // Više tačnih
                const selection = (newAnswers[questionId] as number[] | undefined) || [];
                newAnswers[questionId] = selection.includes(answerValue)
                    ? selection.filter(id => id !== answerValue)
                    : [...selection, answerValue];
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