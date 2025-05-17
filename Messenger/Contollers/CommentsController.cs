using Messenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly MessengerDbContext _context;
        public CommentsController(MessengerDbContext context)
        {
            _context = context;
        }
        [HttpGet("{postId}")]
        public IActionResult GetComments(int postId,int offset=0,int limit=50) 
        {
            if (limit < 0 || offset < 0)
                return BadRequest();
            if (_context.Posts.FirstOrDefault(x => x.Id == postId)==null)
                return BadRequest();
            return Ok(_context.Comments.Where(x=>x.PostId==postId).Skip(offset).Take(Math.Min(50, limit)));
        }
        [HttpPost("{postId}")]
        public IActionResult CreateComment(int postId,[FromBody] CommentRequest request)
        {
            var post= _context.Posts.FirstOrDefault(x=>x.Id== postId);
            if(post == null || _context.Users.FirstOrDefault(x => x.Id == request.UserId) == null)
                return BadRequest();
            if (!AuthController.CheckToken(request.Token, request.UserId))
                return Forbid();
            var comment = new Comment(request.UserId,postId, request.Text);
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return CreatedAtAction(null, comment.Id,comment);
        }
        [HttpPut("{postId}")]
        public IActionResult UpdateComment(int postId, [FromBody] CommentRequest request)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.Id == request.Id);
            if (comment == null)
                return BadRequest();
            if (comment.Id != request.UserId || !AuthController.CheckToken(request.Token, request.UserId))
                return Forbid();
            comment.Text = request.Text; 
            _context.SaveChanges();
            return Ok(comment);
        }
        [HttpDelete("{postId}")]
        public IActionResult DeleteComment(int postId, [FromBody] CommentRequest request)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.Id == request.Id);
            if (comment == null)
                return BadRequest();
            if (comment.Id != request.UserId || !AuthController.CheckToken(request.Token, request.UserId))
                return Forbid();
            _context.Comments.Remove(comment);
            return Ok(comment);
        }
    }
    public class CommentRequest
    {
        public int Id { get; set; }
        public string Token {  get; set; }
        public int UserId {  get; set; }
        public string? Text { get; set; }
    }
}
