import React from 'react';

const formatTime = (seconds: number): string => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
};

interface QuizHeaderProps {
    name: string;
    timeLeft: number;
}

const QuizHeader: React.FC<QuizHeaderProps> = ({ name, timeLeft }) => (
    <div className="quiz-header">
        <h2>{name}</h2>
        <div className="quiz-timer">{formatTime(timeLeft)}</div>
    </div>
);

export default QuizHeader;