
export interface Quiz {
  quizID: number;
  name: string;
  description: string;
  difficulty: string;
  timesCompleted: number;
  maxPoints: number;
  numberOfQuestions: number; 
}