using Microsoft.AspNetCore.Mvc;
using KvizHub.Api.Models;
using KvizHub.Api.Services.Auth;
using KvizHub.Api.Dtos.Auth;

namespace KvizHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _authService.UserExists(registerDto.Username))
                return BadRequest("Username already exists.");

            var userToCreate = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                UserRole = Models.Enums.UserRole.User
            };

            var createdUser = await _authService.Register(userToCreate, registerDto.Password);
            return StatusCode(201); // 201 Created
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto.Username, loginDto.Password);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }
    }
}