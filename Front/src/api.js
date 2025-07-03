const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost/api';

export default {
  AUTH: {
    REGISTER: `${API_BASE_URL}/Auth/register`,
    LOGIN: `${API_BASE_URL}/auth/login`,
    CHECK: `${API_BASE_URL}/auth/check`
  },
  POSTS: {
    BASE: `${API_BASE_URL}/posts`,
    LIKE: (postId) => `${API_BASE_URL}/posts/${postId}/like`,
    COMMENTS: (postId) => `${API_BASE_URL}/Posts/${postId}/Comments`
  },
};