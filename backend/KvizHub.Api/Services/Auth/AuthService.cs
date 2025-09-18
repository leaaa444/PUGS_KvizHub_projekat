using KvizHub.Api.Data;
using KvizHub.Api.Dtos.Auth;
using KvizHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KvizHub.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly KvizHubContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AuthService(KvizHubContext context, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> EmailExists(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        public async Task<string?> Login(string loginIdentifier, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == loginIdentifier.ToLower() ||
                u.Email.ToLower() == loginIdentifier.ToLower()
            );
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
            {
                return null;
            }

            string token = CreateToken(user);
            return token;
        }

        public async Task<User> Register(RegisterDto registerDto)
        {
            string profilePictureUrl = await SaveProfilePicture(registerDto.ProfilePicture);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                ProfilePictureUrl = profilePictureUrl, 
                UserRole = Models.Enums.UserRole.User
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            user.HashedPassword = hashedPassword;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private async Task<string> SaveProfilePicture(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Slika nije poslata.");
            }

            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "ProfilePictures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileExtension = Path.GetExtension(imageFile.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/ProfilePictures/{uniqueFileName}";
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim("profilePictureUrl", user.ProfilePictureUrl ?? string.Empty)

            };

            var keyString = _configuration.GetSection("Jwt:Key").Value;
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("Jwt Key nije podešen u konfiguraciji!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var durationInMinutes = _configuration.GetValue<double>("Jwt:DurationInMinutes");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                Audience = _configuration.GetSection("Jwt:Audience").Value
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}