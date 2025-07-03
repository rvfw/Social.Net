import React from 'react';
import { Box, Typography, IconButton } from '@mui/material';
import { 
  Favorite as FavoriteIcon, 
  FavoriteBorder as FavoriteBorderIcon,
  KeyboardArrowUp as ArrowUpIcon,
  KeyboardArrowDown as ArrowDownIcon 
} from '@mui/icons-material';
import api from '../api';

function PostActions({ post, user, setPosts, posts, showComments, setShowComments }) {
  const handleLike = async (postId) => {
    try {
      const post = posts.find(p => p.id === postId);
      if (!post) return;
      const isLiked = post.likesBy.includes(user.id);

      setPosts(posts.map(p => {
        if (p.id === postId) {
          return {
            ...p,
            likes: isLiked ? p.likes - 1 : p.likes + 1,
            likesBy: isLiked 
              ? p.likesBy.filter(id => id !== user.id)
              : [...p.likesBy, user.id]
          };
        }
        return p;
      }));

      const response = await fetch(api.POSTS.LIKE(postId), {
        method: isLiked ? 'DELETE' : 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error(isLiked ? 'Unlike failed' : 'Like failed');

      const updatedPost = await response.json();
      setPosts(posts.map(p => p.id === postId ? updatedPost : p));

    } catch (error) {
      console.error('Error:', error);
      setPosts([...posts]);
    }
  };

  const toggleComments = () => {
    setShowComments(!showComments);
  };

  return (
    <Box sx={{ display: 'flex', alignItems: 'center' }}>
      <IconButton onClick={() => handleLike(post.id)}>
        {post.likesBy.includes(user.id) ? (
          <FavoriteIcon color="error" />
        ) : (
          <FavoriteBorderIcon />
        )}
      </IconButton>
      <Typography variant="caption" sx={{ mr: 2 }}>
        {post.likes} лайки
      </Typography>
      
      <IconButton onClick={toggleComments}>
        {showComments ? <ArrowUpIcon /> : <ArrowDownIcon />}
      </IconButton>
      <Typography variant="caption">
        {post.commentsCount} комментарии
      </Typography>
    </Box>
  );
}

export default PostActions;