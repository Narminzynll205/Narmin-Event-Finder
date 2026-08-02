using EventFinder.Web.DTOs.Auth;

namespace EventFinder.Web.Services
{
    /// <summary>
    /// Handles user registration, login and JWT token issuance.
    /// </summary>
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors, AuthResponseDto? Result)> RegisterAsync(RegisterDto dto);

        Task<(bool Succeeded, string[] Errors, AuthResponseDto? Result)> LoginAsync(LoginDto dto);
    }
}
