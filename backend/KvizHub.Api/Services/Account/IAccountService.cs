using KvizHub.Api.Dtos.Account;
using Microsoft.AspNetCore.Http;

namespace KvizHub.Api.Services.Account
{
    public interface IAccountService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<UpdateProfileResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<string> UpdateProfilePictureAsync(string userId, IFormFile file);
    }
}