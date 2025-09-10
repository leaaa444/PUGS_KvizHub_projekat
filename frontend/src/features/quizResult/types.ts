export interface QuizResultDetails {
    resultId: number;
    quizName: string;
    score: number;
    maxPossibleScore: number;
    dateCompleted: string; // ISO string datum
    timeTaken: number; // u sekundama
    questions: QuestionResult[];
}

export interface QuestionResult {
    questionId: number;
    text: string;
    type: 'SingleChoice' | 'MultipleChoice' | 'FillInTheBlank' | 'TrueFalse';
    options: OptionResult[];
    userAnswer: {
        answerOptionIds: number[];
        answerText: string | null;
    };
    correctAnswer: {
        answerOptionIds: number[];
        answerText: string | null;
    };
    isCorrect: boolean;
}

export interface OptionResult {
    optionId: number;
    text: string;
}