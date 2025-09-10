import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL + '/Results/'; // Novi URL

interface QuizSubmission {
  quizId: number;
  timeTaken: number;
  answers: AnswerSubmission[];
}

interface AnswerSubmission {
  questionId: number;
  answerOptionIds: number[];
  answerText: string | null;
}

const getResultDetails = (resultId: number) => {
  return axios.get(`${API_URL}${resultId}`, { headers: authHeader() });
};

const submitQuiz = (submissionData: QuizSubmission) => {
  return axios.post(API_URL, submissionData, { headers: authHeader() });
};

const resultService = {
  getResultDetails,
  submitQuiz,
};

export default resultService;