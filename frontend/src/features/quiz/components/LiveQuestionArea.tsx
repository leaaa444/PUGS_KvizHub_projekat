import React from 'react';
import { QuestionDto } from '../../live-quiz/types';

interface LiveQuestionAreaProps {
    currentQuestion: QuestionDto;
    showOptions: boolean;
    currentAnswer: any;
    currentQuestionIndex: number;
    totalQuestions: number;
    onAnswerChange: (value: any) => void;
}

const LiveQuestionArea: React.FC<LiveQuestionAreaProps> = ({
    currentQuestion,
    showOptions,
    currentAnswer,
    currentQuestionIndex,
    totalQuestions,
    onAnswerChange,
}) => {
    
    const handleMultipleChoiceChange = (optionId: number) => {
        const currentSelection = Array.isArray(currentAnswer) ? [...currentAnswer] : [];
        const selectedIndex = currentSelection.indexOf(optionId);

        if (selectedIndex === -1) {
            currentSelection.push(optionId);
        } else {
            currentSelection.splice(selectedIndex, 1);
        }
        
        onAnswerChange(currentSelection);
    };
    

    const renderAnswerOptions = () => {
        if (!showOptions) return null;

        if (currentQuestion.type === 'FillInTheBlank') {
            return (
                <div className="answer-options">
                    <input
                        type="text"
                        value={currentAnswer || ''}
                        onChange={(e) => onAnswerChange(e.target.value)}
                        placeholder="Unesite vaš odgovor"
                        className="text-answer-input"
                    />
                </div>
            );
        }

        return (
            <div className="answer-options">
                {currentQuestion.answerOptions.map((option) => {
                    const isMultipleChoice = currentQuestion.type === 'MultipleChoice';
                    
                    return (
                        <label key={option.answerOptionID} className="answer-label">
                            <input
                                type={isMultipleChoice ? 'checkbox' : 'radio'}
                                name={`question-${currentQuestion.questionID}`}
                                checked={
                                    isMultipleChoice
                                        ? (currentAnswer as number[] | undefined)?.includes(option.answerOptionID) || false
                                        : currentAnswer === option.answerOptionID
                                }
                                onChange={() => 
                                    isMultipleChoice
                                        ? handleMultipleChoiceChange(option.answerOptionID)
                                        : onAnswerChange(option.answerOptionID)
                                }
                            />
                            {option.text}
                        </label>
                    );
                })}
            </div>
        );
    };

    return (
        <div className="question-area-container">
            <div className="question-header">
                <span>Pitanje {currentQuestionIndex + 1} / {totalQuestions}</span>
                <span>{currentQuestion.pointNum} poena</span>
            </div>
            <h2 className="question-text">{currentQuestion.questionText}</h2>
            {renderAnswerOptions()}
        </div>
    );
};

export default LiveQuestionArea;