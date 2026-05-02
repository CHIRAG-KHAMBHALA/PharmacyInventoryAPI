using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryAPI.DTOs;
using PharmacyInventoryAPI.Services;

namespace PharmacyInventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")] // Sirf existing Admin hi naya Admin bana sakta hai
        public async Task<IActionResult> RegisterAdmin(RegisterDto dto)
        {
            var token = await _authService.RegisterAdmin(dto);
            if (token == null)
                return BadRequest("Username already exists.");
            return Ok(new { token });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _authService.Register(dto);
            if (token == null)
                return BadRequest("Username already exists.");
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);
            if (token == null)
                return Unauthorized("Invalid username or password.");
            return Ok(new { token });
        }
    }
}