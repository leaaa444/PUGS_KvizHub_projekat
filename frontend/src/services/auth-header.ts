export default function authHeader() {
  const token = localStorage.getItem('user_token');

  if (token) {
    return { Authorization: 'Bearer ' + token };
  } else {
    return {};
  }
}