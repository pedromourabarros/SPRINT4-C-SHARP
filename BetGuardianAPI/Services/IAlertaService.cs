using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Interface do serviço de alertas
    /// </summary>
    public interface IAlertaService
    {
        Task<IEnumerable<AlertaResponseDTO>> GetAllAlertasAsync();
        Task<AlertaResponseDTO?> GetAlertaByIdAsync(int id);
        Task<IEnumerable<AlertaResponseDTO>> GetAlertasByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<AlertaResponseDTO>> GetAlertasByTipoAsync(TipoAlerta tipo);
        Task<IEnumerable<AlertaResponseDTO>> GetAlertasRecentesAsync(int dias = 7);
        Task<AlertaResponseDTO> CreateAlertaAsync(CreateAlertaDTO createDto);
        Task<AlertaResponseDTO?> UpdateAlertaAsync(int id, UpdateAlertaDTO updateDto);
        Task<bool> DeleteAlertaAsync(int id);
        Task<AlertaResponseDTO> CriarAlertaMotivacionalAsync(int usuarioId, string mensagem);
    }
}
