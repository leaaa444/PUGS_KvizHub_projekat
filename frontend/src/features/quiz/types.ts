export interface AnswerOption {
    answerOptionId: number;
    text: string;
}

export interface Question {
    questionId: number;
    questionText: string;
    type: 'SingleChoice' | 'MultipleChoice' | 'FillInTheBlank' | 'TrueFalse';
    pointNum: number;
    answerOptions: AnswerOption[];
}

export interface QuizData {
    quizId: number;
    name: string;
    timeLimit: number;
    questions: Question[];
}

export type UserAnswers = {
  [key: number]: number | number[] | string | null; // Ključ je ID pitanja, vrednost može biti broj, niz brojeva ili tekst
};
 
export interface ResultData {
  resultId: number;
  score: number;
  maxPossibleScore: number;
  correctAnswers: number;
  totalQuestions: number;
}
