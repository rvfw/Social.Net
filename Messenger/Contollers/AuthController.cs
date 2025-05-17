using Messenger.Models;
using Microsoft.AspNetCore.Identity.Data;
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
    [Route("api/{controller}")]
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

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
{
                new Claim("Id",userByEmail.Id.ToString()),
                new Claim("Password",GetHash(login.Password)),
                new Claim(ClaimTypes.Role, "User")
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    _config.GetValue<double>("Jwt:ExpiryInMinutes")),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if(_context.Users.FirstOrDefault(x=>x.Email==request.Email)!=null)
                return BadRequest();
            var user = new User(request.Username, request.Email, GetHash(request.Password));
            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(null,new { user.Id },user);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public static bool CheckToken(string token,int userId)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            if (jwtToken.Claims.First(x => x.Type == "Id").Value != userId.ToString())
                return false;
            return true;
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
