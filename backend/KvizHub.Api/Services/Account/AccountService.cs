using KvizHub.Api.Dtos.Account;
using KvizHub.Api.Models; 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace KvizHub.Api.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountService(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Korisnik nije pronađen.");
            }

            var userProfileDto = new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return userProfileDto;
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            var usernameExists = await _userManager.Users.AnyAsync(u => u.Username == dto.Username && u.UserID.ToString() != userId);
            if (usernameExists) throw new InvalidOperationException("Korisničko ime je zauzeto.");

            var emailExists = await _userManager.Users.AnyAsync(u => u.Email == dto.Email && u.UserID.ToString() != userId);
            if (emailExists) throw new InvalidOperationException("Email adresa je zauzeta.");

            user.Username = dto.Username;
            user.Email = dto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Greška prilikom ažuriranja profila.");
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("Korisnik nije pronađen.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Greška pri promeni lozinke: {errors}");
            }
        }

        public async Task<string> UpdateProfilePictureAsync(string userId, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(userId);
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
            await _userManager.UpdateAsync(user);

            return fileUrl;
        }
    }
}