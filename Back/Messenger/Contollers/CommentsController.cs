using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api")]
    public class CommentsController : Controller
    {
        private readonly MessengerDbContext _context;
        public CommentsController(MessengerDbContext context)
        {
            _context = context;
        }
        [HttpGet("Posts/{postId}/[controller]")]
        public IActionResult GetComments(int postId,int offset=0,int limit=50) 
        {
            if (limit < 0 || offset < 0)
                return BadRequest();
            if (_context.Posts.FirstOrDefault(x => x.Id == postId)==null)
                return BadRequest();
            var loadedComments = _context.Comments.Include(x=>x.User).Where(x => x.PostId == postId).Skip(offset).Take(Math.Min(50, limit));
            return Ok(loadedComments.OrderByDescending(x=>x.Id));
        }
        [Authorize]
        [HttpPost("Posts/{postId}/[controller]")]
        public IActionResult CreateComment(int postId,[FromBody] CommentRequest request)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var post= _context.Posts.FirstOrDefault(x=>x.Id== postId);
            if(post == null || _context.Users.FirstOrDefault(x => x.Id == userId) == null)
                return BadRequest();
            var comment = new Comment(userId,postId, request.Text);
            _context.Comments.Add(comment);
            post.CommentsCount++;
            _context.SaveChanges();
            return CreatedAtAction(null, comment.Id,_context.Comments.Include(x=>x.User).First(x=>x.Id==comment.Id));
        }
        [Authorize]
        [HttpPut("Posts/{commentId}/[controller]")]
        public IActionResult UpdateComment(int commentId, [FromBody] CommentRequest request)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var comment = _context.Comments.FirstOrDefault(x=>x.Id==commentId);
            if (comment == null)
                return BadRequest();
            if (comment.Id != userId)
                return Forbid();
            comment.Text = request.Text;
            _context.SaveChanges();
            return Ok(comment);
        }
        [Authorize]
        [HttpDelete("Posts/{commentId}/[controller]")]
        public IActionResult DeleteComment(int commentId)
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var comment = _context.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment == null)
                return BadRequest();
            if (comment.Id != userId)
                return Forbid();
            _context.Comments.Remove(comment);
            return Ok(comment);
        }
    }
    public class CommentRequest
    {
        public string Text { get; set; }
    }
}
