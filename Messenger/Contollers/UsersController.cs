using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("{userId}")]
        public IActionResult CreatePost(int userId,[FromBody]PostRequest post)
        {
            var newPost = new Post(post.Text, userId);
            if (!AuthController.CheckToken(post.Token, userId))
                return Forbid();
            _context.Posts.Add(newPost);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUsers), userId,newPost);
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
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId, [FromBody] UserRequest request) { 
            var user = _context.Users.FirstOrDefault(x=>x.Id==userId);
            if(user==null)
                return BadRequest();
            if(!AuthController.CheckToken(request.Token, userId))
                return Forbid();
            foreach(var post in _context.Posts)
                post.LikesBy.Remove(userId);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
    public class UserRequest
    {
        public string Token { get; set; }
    }
}
