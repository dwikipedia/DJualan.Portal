using DJualan.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthDTO = DJualan.Core.DTOs.Auth;

namespace DJualan.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTO.LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            var result = await _authService.AuthenticateAsync(request);
            if (result == null)
            {
                _logger.LogWarning("Failed login for user {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return Ok(result);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDTO.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.RegisterAsync(request);

            if (!success)
                return Conflict(new { message = "Username is already exists / invalid" });

            return Ok(new { message = "Successfully registered!" });
        }
    }
}
