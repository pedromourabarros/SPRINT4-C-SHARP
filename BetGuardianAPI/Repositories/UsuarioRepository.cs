using BetGuardianAPI.Data;
using BetGuardianAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BetGuardianAPI.Repositories
{
    /// <summary>
    /// Implementação do repositório de usuários
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BetGuardianContext _context;

        public UsuarioRepository(BetGuardianContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Alertas)
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Alertas)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Alertas)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetByNivelRiscoAsync(NivelRisco nivelRisco)
        {
            return await _context.Usuarios
                .Include(u => u.Alertas)
                .Where(u => u.NivelRisco == nivelRisco)
                .OrderByDescending(u => u.ValorGasto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosMaiorRiscoAsync(int limit = 10)
        {
            return await _context.Usuarios
                .Include(u => u.Alertas)
                .OrderByDescending(u => u.NivelRisco)
                .ThenByDescending(u => u.ValorGasto)
                .ThenByDescending(u => u.TotalApostas)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }
    }
}
