import React from 'react';
import { Question } from '../types';

interface QuestionAreaProps {
    currentQuestion: Question;
    currentQuestionIndex: number;
    totalQuestions: number;
    currentAnswer: any;
    showOptions: boolean;
    onAnswerChange: (questionId: number, value: any, type: string) => void;
}

const QuestionArea: React.FC<QuestionAreaProps> = ({
    currentQuestion,
    currentQuestionIndex,
    totalQuestions,
    showOptions, 
    currentAnswer,
    onAnswerChange,
}) => {
    const handleCheckboxChange = (optionId: number) => {
        const currentSelectedIds = (currentAnswer as number[] | null) || [];
        let newSelectedIds;

        if (currentSelectedIds.includes(optionId)) {
            newSelectedIds = currentSelectedIds.filter(id => id !== optionId);
        } else {
            newSelectedIds = [...currentSelectedIds, optionId];
        }
        
        onAnswerChange(currentQuestion.questionId, newSelectedIds, currentQuestion.type);
    };
    
    return (
    <div className="quiz-question-area">
        <div className="question-progress">
            Pitanje {currentQuestionIndex + 1} od {totalQuestions}
        </div>
        <h3>{currentQuestion.questionText}</h3>

        {showOptions && (
            <div className="answer-options">
                {currentQuestion.type === 'FillInTheBlank' ? (
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
                                type={currentQuestion.type === 'MultipleChoice' ? 'checkbox' : 'radio'}
                                name={`question-${currentQuestion.questionId}`}
                                checked={
                                    currentQuestion.type === 'MultipleChoice'
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
        )}

    </div>
)};

export default QuestionArea;