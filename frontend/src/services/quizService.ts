import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL + '/Quizzes/';

export interface QuizListData {
  quizID: number;
  name: string;
  description: string;
  difficulty: string;
  maxPoints: number;
  categories: string[]; 
}

const getQuizzes = () => {
  return axios.get(API_URL, { headers: authHeader() });
};

const getQuestionsForQuiz = (quizId: number) => {
  return axios.get(`${API_URL}${quizId}/questions`, { headers: authHeader() });
};

const getQuizById = (id: number) => {
  return axios.get(API_URL + id, { headers: authHeader() });
};

const getQuizForTaker = (id: number) => {
  return axios.get(`${API_URL}${id}/take`, { headers: authHeader() });
};

const createQuizWithQuestions = (quizData: any) => {
  return axios.post(API_URL, quizData, { headers: authHeader() });
};

const deleteQuiz = (id: number) => {
  return axios.delete(API_URL + id, { headers: authHeader() });
};

const updateQuiz = (id: number, quizData: any) => {
  return axios.put(API_URL + id, quizData, { headers: authHeader() });
};

const archiveAndCreateNew = (id: number, quizData: any) => {
  return axios.post(`${API_URL}${id}/archiveAndCreateNew`, quizData, { headers: authHeader() });
};


const quizService = {
  getQuizzes,
  getQuestionsForQuiz,
  createQuizWithQuestions,
  deleteQuiz,
  getQuizById,
  updateQuiz,
  getQuizForTaker,
  archiveAndCreateNew,
};

export default quizService;