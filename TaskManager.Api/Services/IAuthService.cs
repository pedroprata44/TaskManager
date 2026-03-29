using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string username, string password);
        Task<AuthResponse> LoginAsync(string username, string password);
    }
}
