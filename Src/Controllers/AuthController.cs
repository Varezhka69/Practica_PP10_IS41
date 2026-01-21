using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankLoansApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BankLoansApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            Console.WriteLine($"Регистрация: {request.Username}");

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Логин и пароль обязательны");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (existingUser != null)
                return BadRequest("Пользователь уже существует");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                FullName = string.IsNullOrWhiteSpace(request.FullName) ?
                    request.Username : request.FullName,
                Role = "Client",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateToken(user);

            return Ok(new
            {
                success = true,
                message = "Регистрация успешна",
                token = token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    fullName = user.FullName,
                    role = user.Role
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка регистрации: {ex}");
            return StatusCode(500, "Ошибка сервера");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            Console.WriteLine($"Попытка входа: {request.Username}");

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Логин и пароль обязательны");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (user == null)
            {
                Console.WriteLine($"Пользователь не найден: {request.Username}");
                return Unauthorized("Пользователь не найден");
            }

            var inputHash = HashPassword(request.Password);
            Console.WriteLine($"Введенный хеш: {inputHash}");
            Console.WriteLine($"Хеш в БД: {user.PasswordHash}");

            if (inputHash != user.PasswordHash)
            {
                Console.WriteLine("Неверный пароль");
                return Unauthorized("Неверный пароль");
            }

            var token = GenerateToken(user);
            Console.WriteLine($"Токен создан для {user.Username}");

            return Ok(new
            {
                success = true,
                message = "Вход успешен",
                token = token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    fullName = user.FullName,
                    role = user.Role
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка входа: {ex}");
            return StatusCode(500, "Ошибка сервера");
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyHere12345!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: "BankLoansApp",
            audience: "BankLoansAppUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
