using BetGuardianAPI.Models;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Interface do repositório de usuários
    /// </summary>
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetByNivelRiscoAsync(NivelRisco nivelRisco);
        Task<IEnumerable<Usuario>> GetUsuariosMaiorRiscoAsync(int limit = 10);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
