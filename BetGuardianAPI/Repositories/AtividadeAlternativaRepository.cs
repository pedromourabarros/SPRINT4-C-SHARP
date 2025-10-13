using BetGuardianAPI.Data;
using BetGuardianAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Implementação do repositório de atividades alternativas
    /// </summary>
    public class AtividadeAlternativaRepository : IAtividadeAlternativaRepository
    {
        private readonly BetGuardianContext _context;

        public AtividadeAlternativaRepository(BetGuardianContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AtividadeAlternativa>> GetAllAsync()
        {
            return await _context.AtividadesAlternativas
                .OrderBy(a => a.Categoria)
                .ThenBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<AtividadeAlternativa?> GetByIdAsync(int id)
        {
            return await _context.AtividadesAlternativas
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AtividadeAlternativa>> GetByCategoriaAsync(string categoria)
        {
            return await _context.AtividadesAlternativas
                .Where(a => a.Categoria.ToLower() == categoria.ToLower())
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<AtividadeAlternativa>> GetByNivelDificuldadeAsync(int nivelDificuldade)
        {
            return await _context.AtividadesAlternativas
                .Where(a => a.NivelDificuldade == nivelDificuldade)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<AtividadeAlternativa>> GetAtividadesSugeridasAsync(NivelRisco nivelRisco)
        {
            // Lógica para sugerir atividades baseada no nível de risco
            var query = _context.AtividadesAlternativas.AsQueryable();

            switch (nivelRisco)
            {
                case NivelRisco.Baixo:
                    // Para risco baixo, sugerir atividades mais relaxantes
                    query = query.Where(a => a.NivelDificuldade <= 2 && 
                                           (a.Categoria == "Bem-estar" || a.Categoria == "Educação"));
                    break;
                case NivelRisco.Medio:
                    // Para risco médio, sugerir atividades moderadas
                    query = query.Where(a => a.NivelDificuldade <= 3);
                    break;
                case NivelRisco.Alto:
                    // Para risco alto, sugerir atividades mais intensas e sociais
                    query = query.Where(a => a.NivelDificuldade >= 2 && 
                                           (a.Categoria == "Social" || a.Categoria == "Fitness"));
                    break;
                case NivelRisco.Critico:
                    // Para risco crítico, sugerir atividades sociais e de voluntariado
                    query = query.Where(a => a.Categoria == "Social" || a.NivelDificuldade >= 3);
                    break;
            }

            return await query
                .OrderBy(a => a.NivelDificuldade)
                .ThenBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<AtividadeAlternativa> CreateAsync(AtividadeAlternativa atividade)
        {
            _context.AtividadesAlternativas.Add(atividade);
            await _context.SaveChangesAsync();
            return atividade;
        }

        public async Task<AtividadeAlternativa> UpdateAsync(AtividadeAlternativa atividade)
        {
            _context.AtividadesAlternativas.Update(atividade);
            await _context.SaveChangesAsync();
            return atividade;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var atividade = await _context.AtividadesAlternativas.FindAsync(id);
            if (atividade == null)
                return false;

            _context.AtividadesAlternativas.Remove(atividade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AtividadesAlternativas.AnyAsync(a => a.Id == id);
        }
    }
}
