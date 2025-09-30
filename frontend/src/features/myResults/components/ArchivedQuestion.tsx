import React from 'react';

interface ArchivedQuestionProps {
    question: {
        text: string;
        type: 'SingleChoice' | 'MultipleChoice' | 'TrueFalse' | 'FillInTheBlank';
        points: number;
        options: { optionId: number, text: string }[];
        userAnswer: { answerOptionIds: number[], answerText: string | null };
    };
    index: number;
}

const ArchivedQuestion: React.FC<ArchivedQuestionProps> = ({ question, index }) => {
    const selectedOptionIds = new Set(question.userAnswer.answerOptionIds);

    const renderInput = (optionId: number) => {
        const isSelected = selectedOptionIds.has(optionId);
        const inputType = question.type === 'MultipleChoice' ? 'checkbox' : 'radio';

        return <input type={inputType} checked={isSelected} readOnly />;
    };

    return (
        <div className="archived-question-card">
             <header className="archived-question-header">
                <h4>{index}. {question.text}</h4>
                <span className="question-points">
                    {question.points} {question.points === 1 ? 'poen' : 'poena'}
                </span>
            </header>
            <div className="archived-answers-area">
                {question.type === 'FillInTheBlank' ? (
                     <div className="archived-text-answer">
                        <strong>Vaš odgovor:</strong> {question.userAnswer.answerText || 'Niste odgovorili'}
                    </div>
                ) : (
                    <div className="archived-options-list">
                        {question.options.map(option => (
                            <div key={option.optionId} className={`option ${selectedOptionIds.has(option.optionId) ? 'selected' : ''}`}> 
                                {renderInput(option.optionId)}
                                <label>{option.text}</label>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ArchivedQuestion;