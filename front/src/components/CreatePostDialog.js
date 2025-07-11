import React, { useState } from 'react';
import api from '../api';
import { 
  Dialog, 
  DialogTitle, 
  DialogContent, 
  DialogActions, 
  TextField, 
  Button 
} from '@mui/material';

function CreatePostDialog({ open, onClose ,onNewPostCreated }) {
  const [content, setContent] = useState('');
  const handleSubmit = () => {
    const token=localStorage.getItem('token');
    if (!content.trim()) return;
     fetch(api.POSTS.BASE, {
       method: 'POST',
       headers: { 
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`},
       body: JSON.stringify({ Text: content })
     })
     .then(res => res.json())
     .then(newPost => {
        onNewPostCreated(newPost);
       onClose();
       setContent('');
     });
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Новая запись</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label=""
          type="text"
          fullWidth
          multiline
          rows={4}
          value={content}
          onChange={(e) => setContent(e.target.value)}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Отмена</Button>
        <Button onClick={handleSubmit} variant="contained">Опубликовать</Button>
      </DialogActions>
    </Dialog>
  );
}

export default CreatePostDialog;