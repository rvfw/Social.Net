import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api';
import { 
  Box, 
  Container, 
  Paper, 
  Typography, 
  Tabs, 
  Tab, 
  TextField, 
  Button, 
  Alert 
} from '@mui/material';

function AuthPage({onLogin}) {
  const [tabValue, setTabValue] = useState(0);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [username, setName] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      if (tabValue === 0) {
         const response = await fetch(api.AUTH.LOGIN, {
           method: 'POST',
           headers: { 'Content-Type': 'application/json' },
           body: JSON.stringify({ email, password })
         });
         const data = await response.json();
         if (response.ok) {
           onLogin(data.token, data.user);
           navigate('/');
         } else {
           setError(data.message || 'Ошибка входа');
         }
        
      } else {
         const response = await fetch(api.AUTH.REGISTER, {
          method: 'POST',
           headers: { 'Content-Type': 'application/json' },
           body: JSON.stringify({ username, email, password })
         });
         const data = await response.json();
         if (response.ok) {
           onLogin(data.token, data.user);
           navigate('/');
         } else {
           setError(data.message || 'Ошибка регистрации');
         }
      }
    } catch (err) {
      setError('An error occurred. Please try again.');
    }
  };

  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 8 }}>
        <Paper elevation={3} sx={{ p: 4 }}>
          <Typography variant="h4" align="center" gutterBottom>
            {tabValue === 0 ? 'Вход' : 'Регистрация'}
          </Typography>
          
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          
          <Tabs 
            value={tabValue} 
            onChange={(e, newValue) => setTabValue(newValue)} 
            centered
            sx={{ mb: 3 }}
          >
            <Tab label="Вход" />
            <Tab label="Регистрация" />
          </Tabs>
          
          <form onSubmit={handleSubmit}>
            {tabValue === 1 && (
              <TextField
                label="Имя"
                fullWidth
                margin="normal"
                value={username}
                onChange={(e) => setName(e.target.value)}
                required
              />
            )}
            
            <TextField
              label="Почта"
              type="email"
              fullWidth
              margin="normal"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            
            <TextField
              label="Пароль"
              type="password"
              fullWidth
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
            
            <Button 
              type="submit" 
              variant="contained" 
              fullWidth 
              size="large" 
              sx={{ mt: 3 }}
            >
              {tabValue === 0 ? 'Войти' : 'Зарегистрироваться'}
            </Button>
          </form>
        </Paper>
      </Box>
    </Container>
  );
}

export default AuthPage;