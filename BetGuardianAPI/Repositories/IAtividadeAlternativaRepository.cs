using BetGuardianAPI.Models;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Interface do repositório de atividades alternativas
    /// </summary>
    public interface IAtividadeAlternativaRepository
    {
        Task<IEnumerable<AtividadeAlternativa>> GetAllAsync();
        Task<AtividadeAlternativa?> GetByIdAsync(int id);
        Task<IEnumerable<AtividadeAlternativa>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<AtividadeAlternativa>> GetByNivelDificuldadeAsync(int nivelDificuldade);
        Task<IEnumerable<AtividadeAlternativa>> GetAtividadesSugeridasAsync(NivelRisco nivelRisco);
        Task<AtividadeAlternativa> CreateAsync(AtividadeAlternativa atividade);
        Task<AtividadeAlternativa> UpdateAsync(AtividadeAlternativa atividade);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
