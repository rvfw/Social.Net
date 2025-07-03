using Azure.Core;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api")]
    public class LikeController : Controller
    {
        private readonly MessengerDbContext _context;
        public LikeController(MessengerDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost("Posts/{postId}/[controller]")]
        public IActionResult LikePost(int postId)
        {
            var post=_context.Posts.Include(p => p.User).FirstOrDefault(x => x.Id == postId);
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            if (post == null || _context.Users.FirstOrDefault(x => x.Id == userId) == null)
                return BadRequest();
            if (post.LikesBy.Contains(userId))
                return BadRequest();
            post.LikesBy.Add(userId);
            _context.SaveChanges();
            return Ok(post);
        }
        [Authorize]
        [HttpDelete("Posts/{postId}/[controller]")]
        public IActionResult DeleteLike(int postId)
        {
            var post = _context.Posts.Include(p=>p.User).FirstOrDefault(x => x.Id == postId);
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            if (post == null || _context.Users.FirstOrDefault(x => x.Id == userId) == null)
                return BadRequest();
            if (!post.LikesBy.Contains(userId))
                return BadRequest();
            post.LikesBy.Remove(userId);
            _context.SaveChanges();
            return Ok(post);
        }
    }
}
