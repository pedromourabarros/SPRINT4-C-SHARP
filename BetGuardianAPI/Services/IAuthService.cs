using BetGuardianAPI.DTOs;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Interface do serviço de autenticação
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<bool> ValidateUserAsync(string email, string senha);
        string GenerateJwtToken(int userId, string email);
    }
}
