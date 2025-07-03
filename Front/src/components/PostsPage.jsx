import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api';
import { CircularProgress, Container, Box, Button } from '@mui/material';
import PostItem from './PostItem';
import CreatePostDialog from './CreatePostDialog';

function PostsPage({ user }) {
  const [posts, setPosts] = useState([]);
  const [openCreatePost, setOpenCreatePost] = useState(false);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await fetch(api.POSTS.BASE, {
          headers: {
            'Authorization': `Bearer ${user?.token}`
          }
        });
        
        if (response.status === 401) {
          navigate('/auth');
          return;
        }
        
        if (!response.ok) throw new Error('Failed to fetch posts');
        
        const data = await response.json();
        setPosts(data);
      } catch (error) {
        console.error('Error fetching posts:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchPosts();
  }, [user, navigate]);

  const handleNewPostCreated = (newPost) => {
    setPosts(prev => [newPost, ...prev]);
  };
  <CreatePostDialog 
  open={openCreatePost}
  onClose={() => setOpenCreatePost(false)}
  onNewPostCreated={handleNewPostCreated}
  />
  if (loading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'center', mb: 3 }}>
        <Button 
          variant="contained" 
          onClick={() => setOpenCreatePost(true)}
        >
          Новая запись
        </Button>
      </Box>

      {posts.map(post => (
        <PostItem 
          key={post.id} 
          post={post} 
          user={user} 
          setPosts={setPosts} 
          posts={posts} 
        />
      ))}

      <CreatePostDialog 
        open={openCreatePost}
        onClose={() => setOpenCreatePost(false)}
        onNewPostCreated={handleNewPostCreated}
      />
    </Container>
  );
}

export default PostsPage;