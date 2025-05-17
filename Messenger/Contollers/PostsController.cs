using Messenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/{controller}")]
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
            return Ok(_context.Posts.AsQueryable().Skip(offset).Take(Math.Min(50,limit)));
        }
        [HttpPut("{postId}")]
        public IActionResult UpdatePost(int postId, [FromBody] PostRequest post)
        {
            var oldPost = _context.Posts.FirstOrDefault(x => x.Id == postId);
            if (oldPost == null)
                return BadRequest();
            if (oldPost.UserId != post.UserId || !AuthController.CheckToken(post.Token, post.UserId))
                return Forbid();
            oldPost.Text=post.Text;
            _context.SaveChanges();
            return Ok(oldPost);
        }
        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId, [FromBody]PostRequest post)
        {
            var oldPost= _context.Posts.FirstOrDefault(x=>x.Id==postId);
            if(oldPost == null)
                return BadRequest();
            if(oldPost.UserId!= post.UserId || !AuthController.CheckToken(post.Token,post.UserId))
                return Forbid();
            _context.Users.First(x => x.Id == post.UserId).Posts.Remove(oldPost);
            _context.SaveChanges();
            return Ok(oldPost);
        }
    }
    public class PostRequest
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
