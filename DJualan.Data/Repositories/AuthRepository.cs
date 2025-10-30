using BCrypt.Net;
using DJualan.Core.DTOs.Auth;
using DJualan.Core.Interfaces;
using DJualan.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DJualan.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            _logger.LogDebug("Querying user {Username} from database", username);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("User {Username} not found in database", username);
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isValid ? user : null;
        }

        public async Task<bool> CreateUserAsync(RegisterRequest request)
        {
            _logger.LogDebug("Creating a new user {Username} ", request.Username);

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                _logger.LogDebug("Username {Username} is already exists", request.Username);
                return false;
            }

            var newUser = new User
            {
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Successfully created a new user: {Username} ", request.Username);

            return true;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            _logger.LogDebug("fetching username: {Username} ", username);

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
