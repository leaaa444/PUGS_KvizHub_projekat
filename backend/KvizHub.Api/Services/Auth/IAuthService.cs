using KvizHub.Api.Dtos.Auth;
using KvizHub.Api.Models;


namespace KvizHub.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User> Register(RegisterDto registerDto);
        Task<string?> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
