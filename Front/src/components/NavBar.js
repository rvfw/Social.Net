import React from 'react';
import { 
  AppBar, 
  Box,
  Toolbar, 
  Typography, 
  Button, 
  IconButton,
  Avatar 
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

function NavBar({ user, onLogout, onCreatePost }) {
  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          KV
        </Typography>
        
        <Box sx={{ display: 'flex', alignItems: 'center', ml: 2 }}>
          <Avatar sx={{ mr: 1 }}>{user?.name?.charAt(0)}</Avatar>
          <Typography variant="subtitle1" sx={{ mr: 2 }}>
            {user?.name}
          </Typography>
          <Button color="inherit" onClick={onLogout}>Выйти</Button>
        </Box>
      </Toolbar>
    </AppBar>
  );
}

export default NavBar;