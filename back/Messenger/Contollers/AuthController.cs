using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly MessengerDbContext _context;
        public AuthController(IConfiguration config, ILogger<AuthController> logger, MessengerDbContext context)
        {
            _config = config;
            _logger = logger;
            _context = context;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var userByEmail = _context.Users.FirstOrDefault(x => x.Email == login.Email);
            if (userByEmail == null || userByEmail.Password != GetHash(login.Password))
                return BadRequest();

            var token = GenerateToken(userByEmail.Id);
            return Ok(new { token = token,user=userByEmail });
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if(_context.Users.FirstOrDefault(x=>x.Email==request.Email)!=null)
                return BadRequest();
            var user = new User(request.Username, request.Email, GetHash(request.Password));
            _context.Users.Add(user);
            _context.SaveChanges();
            var token = GenerateToken(_context.Users.FirstOrDefault(x => x.Email == user.Email).Id);
            return CreatedAtAction(null,user.Id, new { token, user });
        }
        [Authorize]
        [HttpGet("Check")]
        public IActionResult GetUserByToken()
        {
            var userId = int.Parse(HttpContext.User.Claims.First(x=>x.Type=="Id").Value!);
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return BadRequest();
            return Ok(user);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        [ApiExplorerSettings(IgnoreApi =true)]
        public string GenerateToken(int userId)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id",userId.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var token= new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    _config.GetValue<double>("Jwt:ExpiryInMinutes")),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public class LoginRequest
    {
        public int Id {  get; set; }
        [Required(ErrorMessage = "Text is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Text must be between 5 and 200 characters")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Text is required")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Text must be between 4 and 200 characters")]
        public required string Password { get; set; }
    }
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Text is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Text must be between 3 and 200 characters")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Text is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Text must be between 5 and 200 characters")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Text is required")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Text must be between 4 and 200 characters")]
        public required string Password { get; set; }
    }
}
