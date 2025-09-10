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
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (await _authService.UserExists(registerDto.Username))
                return BadRequest("Korisničko ime već postoji.");

            try
            {
                await _authService.Register(registerDto);
                return StatusCode(201, new { message = "Korisnik uspešno registrovan." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Generalna greška za slučaj da nešto drugo krene po zlu (npr. problem sa čuvanjem fajla)
                return StatusCode(500, new { message = "Došlo je do greške na serveru." });
            }
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