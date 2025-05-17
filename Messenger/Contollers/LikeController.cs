using Azure.Core;
using Messenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/Posts/[controller]")]
    public class LikeController : Controller
    {
        private readonly MessengerDbContext _context;
        public LikeController(MessengerDbContext context)
        {
            _context = context;
        }
        [HttpPut("{postId}")]
        public IActionResult LikePost(int postId, [FromBody] LikeRequest like)
        {
            var post=_context.Posts.FirstOrDefault(x => x.Id == postId);
            if (post == null || _context.Users.FirstOrDefault(x => x.Id == like.UserId) == null)
                return BadRequest();
            if(!AuthController.CheckToken(like.Token, like.UserId))
                return Forbid();
            if (like.SetLike)
            {
                if (post.LikesBy.Contains(like.UserId))
                    return BadRequest();
                post.LikesBy.Add(like.UserId);
            }
            else
            {
                if (!post.LikesBy.Contains(like.UserId))
                    return BadRequest();
                post.LikesBy.Remove(like.UserId);
            }
            _context.SaveChanges();
            return Ok(post);
        }
    }
    public class LikeRequest
    {
        public required string Token { get; set; }
        public required int UserId { get; set; }
        public required bool SetLike { get; set; }
    }
}
