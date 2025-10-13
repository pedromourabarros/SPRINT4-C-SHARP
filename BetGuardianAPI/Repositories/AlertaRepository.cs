using BetGuardianAPI.Data;
using BetGuardianAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Implementação do repositório de alertas
    /// </summary>
    public class AlertaRepository : IAlertaRepository
    {
        private readonly BetGuardianContext _context;

        public AlertaRepository(BetGuardianContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alerta>> GetAllAsync()
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<Alerta?> GetByIdAsync(int id)
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Alerta>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alerta>> GetByTipoAsync(TipoAlerta tipo)
        {
            return await _context.Alertas
                .Include(a => a.Usuario)
                .Where(a => a.Tipo == tipo)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alerta>> GetAlertasRecentesAsync(int dias = 7)
        {
            var dataLimite = DateTime.UtcNow.AddDays(-dias);
            return await _context.Alertas
                .Include(a => a.Usuario)
                .Where(a => a.DataCriacao >= dataLimite)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<Alerta> CreateAsync(Alerta alerta)
        {
            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
            return alerta;
        }

        public async Task<Alerta> UpdateAsync(Alerta alerta)
        {
            _context.Alertas.Update(alerta);
            await _context.SaveChangesAsync();
            return alerta;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
                return false;

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Alertas.AnyAsync(a => a.Id == id);
        }
    }
}
