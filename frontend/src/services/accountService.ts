import axios from 'axios';
import authHeader from './auth-header';

const API_URL = process.env.REACT_APP_API_URL + '/Account/';

const getProfile = () => {
    return axios.get(API_URL + 'profile', { headers: authHeader() });
};

const updateProfile = (profileData: { username: string; email: string }) => {
    return axios.put(API_URL + 'profile', profileData, { headers: authHeader() });
};

const changePassword = (passwordData: { currentPassword: string; newPassword: string }) => {
    return axios.post(API_URL + 'change-password', passwordData, { headers: authHeader() });
};

const updateProfilePicture = (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    return axios.post(API_URL + 'profile-picture', formData, {
        headers: {
            ...authHeader(),
            'Content-Type': 'multipart/form-data',
        },
    });
};

const accountService = {
    getProfile,
    updateProfile,
    changePassword,
    updateProfilePicture,
};

export default accountService;