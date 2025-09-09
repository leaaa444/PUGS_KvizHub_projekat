import React from 'react';

interface QuizNavigationProps {
    currentQuestionIndex: number;
    totalQuestions: number;
    onPrev: () => void;
    onNext: () => void;
    onSubmit: () => void;
}

const QuizNavigation: React.FC<QuizNavigationProps> = ({
    currentQuestionIndex,
    totalQuestions,
    onPrev,
    onNext,
    onSubmit,
}) => (
    <div className="quiz-navigation">
        <button onClick={onPrev} disabled={currentQuestionIndex === 0}>
            Prethodno
        </button>
        {currentQuestionIndex === totalQuestions - 1 ? (
            <button onClick={onSubmit} className="finish-btn">
                Završi Kviz
            </button>
        ) : (
            <button onClick={onNext} disabled={currentQuestionIndex === totalQuestions - 1}>
                Sledeće
            </button>
        )}
    </div>
);

export default QuizNavigation;