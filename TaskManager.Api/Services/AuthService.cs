using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly TaskDbContext _context;
        private readonly IConfiguration? _configuration;

        public AuthService(TaskDbContext context, IConfiguration? configuration = null)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(string email, string username, string password)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");
            
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format");

            // Validate username
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");
            
            if (username.Length < 3)
                throw new ArgumentException("Username must be at least 3 characters");

            // Validate password
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");
            
            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters");

            // Check for duplicate email
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingEmail != null)
                throw new InvalidOperationException("Email already exists");

            // Check for duplicate username
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUsername != null)
                throw new InvalidOperationException("Username already exists");

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponse { Token = token };
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            // Find user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponse { Token = token };
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration?["Jwt:Key"] ?? "YourSuperSecretKeyHere12345678901234567890";
            var issuer = _configuration?["Jwt:Issuer"] ?? "TaskManager";
            var audience = _configuration?["Jwt:Audience"] ?? "TaskManagerUsers";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
    }
}
