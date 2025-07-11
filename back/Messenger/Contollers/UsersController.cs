using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MessengerDbContext _context;
        public UsersController(MessengerDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetUsers(int offset=0,int limit=50)
        {
            if(offset == 0 || limit == 0)
                return BadRequest();
            return Ok(_context.Users.AsQueryable().Skip(offset).Take(Math.Min(50,limit)));
        }
        [HttpGet("{userId}/Posts")]
        public IActionResult GetPostsByUser(int userId,int offset=0,int limit=50)
        {
            if (limit < 0 || offset < 0)
                return BadRequest();
            if (_context.Users.FirstOrDefault(x=>x.Id == userId)==null)
                return BadRequest();
            return Ok(_context.Posts.Where(x => x.UserId == userId).AsQueryable().Skip(offset).Take(Math.Min(50,limit)));
        }
        [Authorize]
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser() {
            var userId = int.Parse(HttpContext.User.Claims.First(x => x.Type == "Id").Value!);
            var user = _context.Users.FirstOrDefault(x=>x.Id==userId);
            foreach (var post in _context.Posts)
            {
                post.LikesBy.Remove(userId);
                post.CommentsCount -= post.Comments.Where(c => c.UserId == userId).Count();
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
