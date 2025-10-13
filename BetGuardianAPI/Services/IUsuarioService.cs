using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Interface do serviço de usuários
    /// </summary>
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponseDTO>> GetAllUsuariosAsync();
        Task<UsuarioResponseDTO?> GetUsuarioByIdAsync(int id);
        Task<UsuarioResponseDTO> CreateUsuarioAsync(CreateUsuarioDTO createDto);
        Task<UsuarioResponseDTO?> UpdateUsuarioAsync(int id, UpdateUsuarioDTO updateDto);
        Task<bool> DeleteUsuarioAsync(int id);
        Task<AnaliseRiscoDTO> AnalisarRiscoUsuarioAsync(int id);
        Task<IEnumerable<UsuarioResponseDTO>> GetUsuariosMaiorRiscoAsync(int limit = 10);
        Task<IEnumerable<UsuarioResponseDTO>> GetUsuariosPorNivelRiscoAsync(NivelRisco nivelRisco);
    }
}
