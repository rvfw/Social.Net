import React, { useState } from 'react';
import { Card, CardContent, Typography, Avatar, Box } from '@mui/material';
import PostActions from './PostActions';
import PostComments from './PostComments';

function PostItem({ post, user, setPosts, posts }) {
  const [newComment, setNewComment] = useState('');
  const [showComments, setShowComments] = useState(false);

  return (
    <Card sx={{ mb: 3 }}>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Avatar sx={{ mr: 1 }}>{post.user.name.charAt(0)}</Avatar>
          <Typography variant="subtitle1">
            {post.user.name}
          </Typography>
        </Box>
        
        <Typography 
          paragraph 
          sx={{ 
            mb: 2,
            whiteSpace: 'pre-line',
            wordWrap: 'break-word',
            overflowWrap: 'break-word'
          }}
        >
          {post.text}
        </Typography>
        
        <PostActions 
          post={post}
          user={user}
          setPosts={setPosts}
          posts={posts}
          showComments={showComments}
          setShowComments={setShowComments}
        />
        
        {showComments && (
          <PostComments 
            post={post}
            user={user}
            newComment={newComment}
            setNewComment={setNewComment}
            setPosts={setPosts}
            posts={posts}
          />
        )}
      </CardContent>
    </Card>
  );
}

export default PostItem;