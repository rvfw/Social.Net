import React, { useState, useEffect } from 'react';
import { 
  Box, 
  TextField, 
  Button, 
  Divider, 
  CircularProgress 
} from '@mui/material';
import { Avatar, Typography } from '@mui/material';
import api from '../api';

function PostComments({ post, user, newComment, setNewComment, setPosts, posts }) {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(false);

  const loadComments = async () => {
    setLoading(true);
    try {
      const response = await fetch(api.POSTS.COMMENTS(post.id), {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      
      if (!response.ok) throw new Error('Failed to load comments');
      
      const data = await response.json();
      setComments(data);
    } catch (error) {
      console.error('Error loading comments:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddComment = async () => {
    if (!newComment.trim()) return;
    
    try {
      const response = await fetch(api.POSTS.COMMENTS(post.id), {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({ text: newComment })
      });

      if (!response.ok) throw new Error('Failed to add comment');

      setPosts(posts.map(p => {
        if (p.id === post.id) {
          return {
            ...p,
            commentsCount: p.commentsCount + 1
          };
        }
        return p;
      }));

      setNewComment('');
      await loadComments();

    } catch (error) {
      console.error('Error adding comment:', error);
    }
  };

  useEffect(() => {
    if (post.id) {
      loadComments();
    }
  }, [post.id]);

  return (
    <>
      <Box sx={{ mt: 2 }}>
        <TextField
          fullWidth
          variant="outlined"
          placeholder=""
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          sx={{ mb: 1 }}
        />
        <Button 
          variant="contained" 
          size="small" 
          onClick={handleAddComment}
        >
          Post Comment
        </Button>
      </Box>
      
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <CircularProgress size={24} />
        </Box>
      ) : (
        comments.length > 0 && (
          <Box sx={{ mt: 2 }}>
            <Divider sx={{ mb: 2 }} />
            {comments.map(comment => (
              <Box key={comment.id} sx={{ mb: 2, ml: 2 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  <Avatar sx={{ width: 24, height: 24, mr: 1, fontSize: '0.8rem' }}>
                    {comment.user.name.charAt(0)}
                  </Avatar>
                  <Typography variant="subtitle2" sx={{ mr: 1 }}>
                    {comment.user.name}
                  </Typography>
                </Box>
                <Typography variant="body2" sx={{ ml: 4 }}>
                  {comment.text}
                </Typography>
              </Box>
            ))}
          </Box>
        )
      )}
    </>
  );
}

export default PostComments;