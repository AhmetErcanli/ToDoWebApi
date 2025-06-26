using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.Models;
using ToDoWebApi.Repositories;
using ToDoWebApi.Services;

namespace ToDoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly TokenService _tokenService;
        private readonly AuthService _authService;

        public AuthController(UserRepository userRepo, TokenService tokenService, AuthService authService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Kullanıcı adı ve şifre boş olamaz!");
            if (_userRepo.GetByUsername(req.Username) != null)
                return BadRequest("Kullanıcı zaten var");

            // Sadece 'Admin' ve 'User' rolleri kabul edilsin
            var allowedRoles = new[] { "Admin", "User" };
            var role = req.Role ?? "User";
            if (!allowedRoles.Contains(role))
                return BadRequest("Geçersiz rol. Sadece 'Admin' veya 'User' olabilir.");

            _authService.CreatePasswordHash(req.Password, out var hash, out var salt);
            var user = new User
            {
                Username = req.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role
            };
            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(1);
            _userRepo.Add(user);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Kullanıcı adı ve şifre boş olamaz!");
            var user = _userRepo.GetByUsername(req.Username);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı");
            if (user.PasswordHash == null || user.PasswordSalt == null)
                return StatusCode(500, "Kullanıcı şifre verisi eksik veya bozuk. Lütfen kullanıcıyı yeniden kaydedin.");
            if (!_authService.VerifyPassword(req.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Şifre yanlış");

            var token = _tokenService.CreateToken(user);
            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(1);
            _userRepo.Update(user);
            return Ok(new
            {
                token,
                refreshToken = user.RefreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest req)
        {
            var user = _userRepo.GetByRefreshToken(req.RefreshToken);
            if (user == null || user.RefreshTokenExpireDate < DateTime.UtcNow)
                return Unauthorized("Refresh token geçersiz");
            var token = _tokenService.CreateToken(user);
            user.RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddHours(1);
            _userRepo.Update(user);
            return Ok(new
            {
                token,
                refreshToken = user.RefreshToken
            });
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string? Role { get; set; } // Admin/User
        }
        public class LoginRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }
        public class RefreshRequest
        {
            public string RefreshToken { get; set; } = null!;
        }
    }
} 