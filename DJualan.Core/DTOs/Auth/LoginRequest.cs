using System.ComponentModel.DataAnnotations;

namespace DJualan.Core.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username cannot be empty")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is mandatory")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
