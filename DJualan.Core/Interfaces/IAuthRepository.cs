using DJualan.Core.Models;
using DJualan.Core.DTOs.Auth;

namespace DJualan.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<bool> CreateUserAsync(RegisterRequest request);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
