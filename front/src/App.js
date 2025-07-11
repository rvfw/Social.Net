import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { blue, indigo } from '@mui/material/colors';
import api from '../src/api'
import AuthPage from './components/AuthPage';
import PostsPage from './components/PostsPage';
import NavBar from './components/NavBar';
import CreatePostDialog from './components/CreatePostDialog';
const theme = createTheme({
  palette: {
    primary: blue,
    secondary: indigo,
    mode: 'light',
  },
});
function getCookie(name) {
    let matches = document.cookie.match(new RegExp("(^| )" + name.replace(/([.$?*|{}()[\]\\/+^])/g, "\\$1") + "=([^;]+)"));
    return matches ? decodeURIComponent(matches[2]) : undefined;
}
function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [openCreatePost, setOpenCreatePost] = useState(false);

  useEffect(() => {
    //const token = getCookie('token');
    const token=localStorage.getItem('token');
    if (token) {
       fetch(api.AUTH.CHECK, {
  method: 'GET',
  headers: { 
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
.then(res => {
  if (res.status === 401) {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    setUser(null);
    throw new Error('Session expired');
  }
  if (!res.ok) {
    throw new Error('Auth check failed');
  }
  return res.json();
})
.then(data => {
  setIsAuthenticated(true);
  setUser(data);
})
.catch(error => {
  console.error('Auth check error:', error);
});
    }
  }, []);

  const handleLogin = (token, userData) => {
    let expirationDate = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);
    //document.cookie = 'token='+token+'; expires=' + expirationDate.toUTCString() + "; path=/; Secure";
    localStorage.setItem('token',token);
    setIsAuthenticated(true);
    setUser(userData);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    setUser(null);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        {isAuthenticated && (
          <NavBar 
            user={user} 
            onLogout={handleLogout} 
            onCreatePost={() => setOpenCreatePost(true)} 
          />
        )}
        
        <Routes>
          <Route 
            path="/" 
            element={
              isAuthenticated ? (
                <PostsPage user={user} />
              ) : (
                <Navigate to="/auth" replace />
              )
            } 
          />
          <Route 
            path="/auth" 
            element={
              !isAuthenticated ? (
                <AuthPage onLogin={handleLogin} />
              ) : (
                <Navigate to="/" replace />
              )
            } 
          />
        </Routes>

        {isAuthenticated && (
          <CreatePostDialog 
            open={openCreatePost} 
            onClose={() => setOpenCreatePost(false)} 
            userId={user?.id} 
          />
        )}
      </Router>
    </ThemeProvider>
  );
}

export default App;