export interface GameRoom {
    roomCode: string;
    quizId: number;
    quizName?: string;
    hostUsername: string;
    status: 'Lobby' | 'InProgress' | 'Finished'; 
    players: Player[];
    currentQuestionIndex?: number;
    currentQuestionStartTime?: string;
}

export interface Player {
    username: string;
    score: number;
    imageUrl?: string; 
    isDisconnected?: boolean;
}

export interface AnswerOptionDto {
    answerOptionID: number;
    text: string;
}

export interface QuestionDto {
    questionID: number;
    type: 'SingleChoice' | 'MultipleChoice' | 'FillInTheBlank' | 'TrueFalse';
    pointNum: number;
    questionText: string;
    timeLimit: number;
    answerOptions: AnswerOptionDto[];
    totalQuestions: number;
    currentQuestionIndex: number;
    currentQuestionStartTime?: string;
    remainingTime?: number; 
}