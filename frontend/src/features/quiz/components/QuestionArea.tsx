import React from 'react';
import { Question } from '../types';

interface QuestionAreaProps {
    currentQuestion: Question;
    currentQuestionIndex: number;
    totalQuestions: number;
    currentAnswer: any;
    onAnswerChange: (questionId: number, value: any, type: number) => void;
}

const QuestionArea: React.FC<QuestionAreaProps> = ({
    currentQuestion,
    currentQuestionIndex,
    totalQuestions,
    currentAnswer,
    onAnswerChange,
}) => (
    <div className="quiz-question-area">
        <div className="question-progress">
            Pitanje {currentQuestionIndex + 1} od {totalQuestions}
        </div>
        <h3>{currentQuestion.questionText}</h3>
        <div className="answer-options">
            {currentQuestion.type === 3 ? (
                <input
                    type="text"
                    className="text-answer-input"
                    placeholder="Unesite vaš odgovor"
                    value={currentAnswer || ''}
                    onChange={(e) => onAnswerChange(currentQuestion.questionId, e.target.value, currentQuestion.type)}
                />
            ) : (
                currentQuestion.answerOptions.map(option => (
                    <label key={option.answerOptionId} className="answer-label">
                        <input
                            type={currentQuestion.type === 1 ? 'checkbox' : 'radio'}
                            name={`question-${currentQuestion.questionId}`}
                            checked={
                                currentQuestion.type === 1
                                    ? (currentAnswer as number[] | undefined)?.includes(option.answerOptionId) || false
                                    : currentAnswer === option.answerOptionId
                            }
                            onChange={() => onAnswerChange(currentQuestion.questionId, option.answerOptionId, currentQuestion.type)}
                        />
                        {option.text}
                    </label>
                ))
            )}
        </div>
    </div>
);

export default QuestionArea;