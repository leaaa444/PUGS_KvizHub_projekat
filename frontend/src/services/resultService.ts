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

const getMyResults = () => {
  return axios.get(`${API_URL}my-results`, { headers: authHeader() });
};

const getArchivedResultDetails = (resultId: number) => {
  return axios.get(`${API_URL}history/${resultId}`, { headers: authHeader() });
};

const getQuizProgress = (quizId: number) => {
  return axios.get(`${API_URL}progress/${quizId}`, { headers: authHeader() });
};

const getAllRankings = (startDate?: string, endDate?: string) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return axios.get(`${API_URL}all-rankings?${params.toString()}`, { headers: authHeader() });
};

const getGlobalRanking = (startDate?: string, endDate?: string) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

  return axios.get(`${API_URL}global-ranking?${params.toString()}`, { headers: authHeader() });
};

const getAllAdminResults = (
    pageNumber: number,
    pageSize: number,
    username?: string,
    quizId?: number
) => {
    const params = new URLSearchParams();
    params.append('pageNumber', pageNumber.toString());
    params.append('pageSize', pageSize.toString());

    if (username) {
        params.append('username', username);
    }
    if (quizId) {
        params.append('quizId', quizId.toString());
    }

    return axios.get(`${API_URL}all-admin-results?${params.toString()}`, { 
        headers: authHeader() 
    });
};


const resultService = {
  getResultDetails,
  submitQuiz,
  getMyResults,
  getArchivedResultDetails,
  getQuizProgress,
  getAllRankings,
  getGlobalRanking,
  getAllAdminResults,

};

export default resultService;