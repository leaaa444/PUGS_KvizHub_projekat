using KvizHub.Api.Data;
using KvizHub.Api.Dtos.Account;
using KvizHub.Api.Models;
using KvizHub.Api.Services.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly KvizHubContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuthService _authService;

        public AccountService(KvizHubContext context, IWebHostEnvironment webHostEnvironment, IAuthService authService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _authService = authService;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
            {
                throw new ArgumentException("ID korisnika nije validan.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            return new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<UpdateProfileResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            if (!int.TryParse(userId, out int id)) throw new ArgumentException("ID korisnika nije validan.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username && u.UserID != id);
            if (usernameExists) throw new InvalidOperationException("Korisničko ime je zauzeto.");

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserID != id);
            if (emailExists) throw new InvalidOperationException("Email adresa je zauzeta.");

            user.Username = dto.Username;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            var newToken = _authService.CreateToken(user);

            return new UpdateProfileResponseDto
            {
                Message = "Profil uspešno ažuriran.",
                NewToken = newToken
            };
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            if (!int.TryParse(userId, out int id)) throw new ArgumentException("ID korisnika nije validan.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.HashedPassword))
            {
                throw new InvalidOperationException("Trenutna lozinka nije tačna.");
            }

            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
        }

        public async Task<string> UpdateProfilePictureAsync(string userId, IFormFile file)
        {
            if (!int.TryParse(userId, out int id))
            {
                throw new ArgumentException("ID korisnika nije validan.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (file == null || file.Length == 0) throw new ArgumentException("Fajl nije poslat.");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/images/profiles/{uniqueFileName}";
            user.ProfilePictureUrl = fileUrl;

            await _context.SaveChangesAsync();

            return fileUrl;
        }
    }
}