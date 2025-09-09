export interface AnswerOption {
    answerOptionId: number;
    text: string;
}

export interface Question {
    questionId: number;
    questionText: string;
    type: number; // 0=Jedan tačan, 1=Više tačnih, 3=Unos teksta
    pointNum: number;
    answerOptions: AnswerOption[];
}

export interface QuizData {
    quizId: number;
    name: string;
    timeLimit: number;
    questions: Question[];
}
 
export interface ResultData {
  resultId: number;
  score: number;
  maxPossibleScore: number;
  correctAnswers: number;
  totalQuestions: number;
}
