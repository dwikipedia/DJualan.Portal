using DJualan.Core.DTOs.Auth;
using DJualan.Core.Interfaces;
using DJualan.Service.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DJualan.Service.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _repo;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository repo, IOptions<JwtSettings> jwtOptions, ILogger<AuthService> logger)
        {
            _repo = repo;
            _jwtSettings = jwtOptions.Value;
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
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            _logger.LogInformation(
               "User {Username} authenticated successfully (token valid {Lifetime} min, expires {Expiration})",
               request.Username,
               _jwtSettings.TokenLifetimeMinutes,
               expiration);

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
