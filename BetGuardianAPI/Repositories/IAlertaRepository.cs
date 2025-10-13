using BetGuardianAPI.Models;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Interface do repositório de alertas
    /// </summary>
    public interface IAlertaRepository
    {
        Task<IEnumerable<Alerta>> GetAllAsync();
        Task<Alerta?> GetByIdAsync(int id);
        Task<IEnumerable<Alerta>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Alerta>> GetByTipoAsync(TipoAlerta tipo);
        Task<IEnumerable<Alerta>> GetAlertasRecentesAsync(int dias = 7);
        Task<Alerta> CreateAsync(Alerta alerta);
        Task<Alerta> UpdateAsync(Alerta alerta);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
