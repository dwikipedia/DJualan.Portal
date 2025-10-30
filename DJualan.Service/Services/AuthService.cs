using DJualan.Core.DTOs.Auth;
using DJualan.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DJualan.Service.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository repo, IConfiguration config, ILogger<AuthService> logger)
        {
            _repo = repo;
            _config = config;
            _logger = logger;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            _logger.LogDebug("Authenticating user {Username}", request.Username);

            var user = await _repo.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for {Username}", request.Username);
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            _logger.LogInformation("User {Username} authenticated successfully", request.Username);

            return new LoginResponse
            {
                Token = jwt,
                Username = user.Username,
                Role = user.Role,
                Expiration = tokenDescriptor.Expires!.Value
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            return await _repo.CreateUserAsync(request);
        }
    }
}
