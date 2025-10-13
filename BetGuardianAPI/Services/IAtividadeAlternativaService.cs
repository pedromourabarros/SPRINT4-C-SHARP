using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Interface do serviço de atividades alternativas
    /// </summary>
    public interface IAtividadeAlternativaService
    {
        Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAllAtividadesAsync();
        Task<AtividadeAlternativaResponseDTO?> GetAtividadeByIdAsync(int id);
        Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAtividadesByCategoriaAsync(string categoria);
        Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAtividadesByNivelDificuldadeAsync(int nivelDificuldade);
        Task<AtividadeAlternativaResponseDTO> CreateAtividadeAsync(CreateAtividadeAlternativaDTO createDto);
        Task<AtividadeAlternativaResponseDTO?> UpdateAtividadeAsync(int id, UpdateAtividadeAlternativaDTO updateDto);
        Task<bool> DeleteAtividadeAsync(int id);
        Task<SugestaoAtividadeDTO> GetSugestaoAtividadesParaUsuarioAsync(int usuarioId);
    }
}
