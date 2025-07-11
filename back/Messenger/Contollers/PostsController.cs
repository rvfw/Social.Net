using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly MessengerDbContext _context;
        public PostsController(MessengerDbContext context)
        {
            _context = context;
        }
        [HttpGet("{postId}")]
        public IActionResult GetPost(int postId)
        {
            var post=_context.Posts.FirstOrDefault(x => x.Id == postId);
            if (post == null)
                return BadRequest();
            return Ok(post);
        }
        [HttpGet]
        public IActionResult GetPosts(int offset=0,int limit=50)
        {
            if(limit<0 || offset<0)
                return BadRequest();
            var result = _context.Posts.Include(p => p.User).AsQueryable()
                .Skip(offset).Take(Math.Min(50, limit)).ToList();
            foreach (var post in result)
                post.CommentsCount = _context.Comments.Include(x => x.User).Where(x => x.PostId == post.Id).Count();
            return Ok(result.ToList().OrderByDescending(x=>x.Id));
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreatePost([FromBody] PostRequest request)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var newPost = new Post(request.Text, userId);
            _context.Posts.Add(newPost);
            _context.SaveChanges();
            var createdPost = _context.Posts.Include(x => x.User).First(x => x.Id == newPost.Id);
            createdPost.CommentsCount = _context.Comments.Include(x => x.User).Where(x => x.PostId == createdPost.Id).Count();
            return CreatedAtAction(nameof(GetPosts), userId,createdPost);
        }
        [Authorize]
        [HttpPut("{postId}")]
        public IActionResult UpdatePost(int postId, [FromBody] PostRequest post)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var oldPost = _context.Posts.FirstOrDefault(x => x.Id == postId);
            if (oldPost == null)
                return BadRequest();
            if (oldPost.UserId != userId)
                return Forbid();
            oldPost.Text = post.Text;
            _context.SaveChanges();
            return Ok(oldPost);
        }
        [Authorize]
        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId, [FromBody] PostRequest post)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var oldPost = _context.Posts.FirstOrDefault(x => x.Id == postId);
            if (oldPost == null)
                return BadRequest();
            if (oldPost.UserId != userId)
                return Forbid();
            _context.Users.First(x => x.Id == userId).Posts.Remove(oldPost);
            _context.SaveChanges();
            return Ok(oldPost);
        }
    }
    public class PostRequest
    {
        public string? Text { get; set; }
    }
}
