using KvizHub.Api.Models;


namespace KvizHub.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User> Register(User user, string password);
        Task<string?> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
