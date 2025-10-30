using System.ComponentModel.DataAnnotations;

namespace DJualan.Core.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username must be filled")]
        [MaxLength(100, ErrorMessage = "Username maksimal 100 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password must be filled")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password needs at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password")]
        [Compare("Password", ErrorMessage = "Password is unmatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "User";
    }
}
