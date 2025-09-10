import React from 'react';
import { QuestionResult } from './types';

interface QuestionReviewProps {
    question: QuestionResult;
    index: number;
}

const QuestionReview: React.FC<QuestionReviewProps> = ({ question, index }) => {
    
    const isOptionCorrect = (optionId: number) => question.correctAnswer.answerOptionIds.includes(optionId);
    const isOptionSelected = (optionId: number) => question.userAnswer.answerOptionIds.includes(optionId);

    const getOptionClassName = (optionId: number) => {
        const isCorrect = isOptionCorrect(optionId);
        const isSelected = isOptionSelected(optionId);

        if (isCorrect) return 'option correct';
        if (isSelected && !isCorrect) return 'option incorrect';
        return 'option';
    };

    return (
        <div className="question-review-card">
            <div className={`question-header`}>
                <h4>{index}. {question.text}</h4>
                    {question.userAnswer.answerOptionIds.length === 0 && (
                        <p className="not-answered-message">Niste odgovorili na ovo pitanje.</p>
                    )}
                <span className={`question-status-tag ${question.isCorrect ? 'correct' : 'incorrect'}`}>
                    {question.isCorrect ? 'Tačno' : 'Netačno'}
                </span>
            </div>
            
            <div className="answers-area">
                {question.type !== 'FillInTheBlank' ? (
                    <div className="options-list">
                        {question.options.map(option => (
                            <div key={option.optionId} className={getOptionClassName(option.optionId)}>
                                {option.text}
                            </div>
                        ))}
                    </div>
                ) : (
                    <div className="text-input-review">
                        <div className={`text-answer ${question.isCorrect ? 'correct' : 'incorrect'}`}>
                            <strong>Vaš odgovor:</strong> {question.userAnswer.answerText || 'Niste odgovorili'}
                        </div>
                        {!question.isCorrect && (
                             <div className="text-answer correct">
                                <strong>Tačan odgovor:</strong> {question.correctAnswer.answerText}
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default QuestionReview;