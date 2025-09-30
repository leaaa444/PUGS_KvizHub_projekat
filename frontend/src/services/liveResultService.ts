import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL + '/LiveResults/'; 

const getGlobalRanking = (startDate?: string, endDate?: string) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return axios.get(`${API_URL}global?${params.toString()}`, { headers: authHeader() });
};

const getFinishedArenas = () => {
    return axios.get(`${API_URL}history`, { headers: authHeader() });
};

const getArenaDetails = (roomCode: string) => {
    return axios.get(`${API_URL}${roomCode}`, { headers: authHeader() });
};

const getMyFinishedArenas = () => {
    return axios.get(`${API_URL}my-history`, { headers: authHeader() });
};

const liveResultService = {
    getGlobalRanking,
    getFinishedArenas,
    getArenaDetails,
    getMyFinishedArenas,
};

export default liveResultService;