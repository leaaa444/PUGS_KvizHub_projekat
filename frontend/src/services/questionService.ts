import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL;

interface CreateAnswerOptionDto {
  text: string;
  isCorrect: boolean;
}

interface CreateQuestionDto {
  questionText: string;
  pointNum: number;
  type: number; // 0=SingleChoice, 1=MultipleChoice, ...
  correctTextAnswer?: string;
  answerOptions: CreateAnswerOptionDto[];
}

const getQuestionsForQuiz = (quizId: number) => {
  return axios.get(`${API_URL}/Quizzes/${quizId}/questions`, { headers: authHeader() });
};

const addQuestionToQuiz = (quizId: number, questionData: CreateQuestionDto) => {
  return axios.post(`${API_URL}/Quizzes/${quizId}/questions`, questionData, { headers: authHeader() });
};

const questionService = {
  getQuestionsForQuiz,
  addQuestionToQuiz,
};

export default questionService;