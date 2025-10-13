using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Repositories;
using BCrypt.Net;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Implementação do serviço de usuários
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAlertaRepository _alertaRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, IAlertaRepository alertaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _alertaRepository = alertaRepository;
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> GetAllUsuariosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(MapToResponseDTO);
        }

        public async Task<UsuarioResponseDTO?> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario != null ? MapToResponseDTO(usuario) : null;
        }

        public async Task<UsuarioResponseDTO> CreateUsuarioAsync(CreateUsuarioDTO createDto)
        {
            // Verificar se email já existe
            var existingUsuario = await _usuarioRepository.GetByEmailAsync(createDto.Email);
            if (existingUsuario != null)
            {
                throw new InvalidOperationException("Já existe um usuário com este email.");
            }

            // Criptografa a senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Senha);

            var usuario = new Usuario
            {
                Nome = createDto.Nome,
                Email = createDto.Email,
                Idade = createDto.Idade,
                PasswordHash = passwordHash,
                TotalApostas = createDto.TotalApostas,
                ValorGasto = createDto.ValorGasto,
                NivelRisco = CalcularNivelRisco(createDto.ValorGasto, createDto.TotalApostas)
            };

            var createdUsuario = await _usuarioRepository.CreateAsync(usuario);
            return MapToResponseDTO(createdUsuario);
        }

        public async Task<UsuarioResponseDTO?> UpdateUsuarioAsync(int id, UpdateUsuarioDTO updateDto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return null;

            // Verificar se email já existe em outro usuário
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != usuario.Email)
            {
                var existingUsuario = await _usuarioRepository.GetByEmailAsync(updateDto.Email);
                if (existingUsuario != null)
                {
                    throw new InvalidOperationException("Já existe um usuário com este email.");
                }
            }

            // Atualizar propriedades
            if (!string.IsNullOrEmpty(updateDto.Nome))
                usuario.Nome = updateDto.Nome;
            if (!string.IsNullOrEmpty(updateDto.Email))
                usuario.Email = updateDto.Email;
            if (updateDto.Idade.HasValue)
                usuario.Idade = updateDto.Idade.Value;
            if (updateDto.TotalApostas.HasValue)
                usuario.TotalApostas = updateDto.TotalApostas.Value;
            if (updateDto.ValorGasto.HasValue)
                usuario.ValorGasto = updateDto.ValorGasto.Value;

            // Recalcular nível de risco
            usuario.NivelRisco = CalcularNivelRisco(usuario.ValorGasto, usuario.TotalApostas);

            var updatedUsuario = await _usuarioRepository.UpdateAsync(usuario);
            return MapToResponseDTO(updatedUsuario);
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            return await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<AnaliseRiscoDTO> AnalisarRiscoUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado.");

            var nivelRiscoCalculado = CalcularNivelRisco(usuario.ValorGasto, usuario.TotalApostas);
            var alertasGerados = new List<string>();

            // Geramos alertas baseados na análise para ajudar o usuário
            if (nivelRiscoCalculado >= NivelRisco.Alto)
            {
                alertasGerados.Add("Nível de risco alto detectado. Considere buscar ajuda profissional.");
            }

            if (usuario.ValorGasto > 1000)
            {
                alertasGerados.Add("Valor gasto em apostas muito alto. Recomenda-se pausa nas atividades de apostas.");
            }

            if (usuario.TotalApostas > 50)
            {
                alertasGerados.Add("Frequência de apostas muito alta. Considere atividades alternativas.");
            }

            var recomendacao = GerarRecomendacao(nivelRiscoCalculado, usuario.ValorGasto, usuario.TotalApostas);

            return new AnaliseRiscoDTO
            {
                UsuarioId = usuario.Id,
                NomeUsuario = usuario.Nome,
                NivelRiscoAtual = usuario.NivelRisco,
                NivelRiscoCalculado = nivelRiscoCalculado,
                ValorGasto = usuario.ValorGasto,
                TotalApostas = usuario.TotalApostas,
                Recomendacao = recomendacao,
                AlertasGerados = alertasGerados
            };
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> GetUsuariosMaiorRiscoAsync(int limit = 10)
        {
            var usuarios = await _usuarioRepository.GetUsuariosMaiorRiscoAsync(limit);
            return usuarios.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> GetUsuariosPorNivelRiscoAsync(NivelRisco nivelRisco)
        {
            var usuarios = await _usuarioRepository.GetByNivelRiscoAsync(nivelRisco);
            return usuarios.Select(MapToResponseDTO);
        }

        private static NivelRisco CalcularNivelRisco(decimal valorGasto, int totalApostas)
        {
            // Este método calcula o risco do usuário com base nas apostas
            // Valores altos indicam maior necessidade de apoio
            if (valorGasto >= 5000 || totalApostas >= 100)
                return NivelRisco.Critico;
            else if (valorGasto >= 2000 || totalApostas >= 50)
                return NivelRisco.Alto;
            else if (valorGasto >= 500 || totalApostas >= 20)
                return NivelRisco.Medio;
            else
                return NivelRisco.Baixo;
        }

        private static string GerarRecomendacao(NivelRisco nivelRisco, decimal valorGasto, int totalApostas)
        {
            return nivelRisco switch
            {
                NivelRisco.Baixo => "Continue monitorando suas atividades. Mantenha o controle sobre suas apostas.",
                NivelRisco.Medio => "Considere reduzir a frequência e valor das apostas. Explore atividades alternativas.",
                NivelRisco.Alto => "Recomenda-se pausa nas apostas e busca por atividades alternativas. Considere apoio profissional.",
                NivelRisco.Critico => "Busque ajuda profissional imediatamente. Considere grupos de apoio e tratamento especializado.",
                _ => "Monitore suas atividades regularmente."
            };
        }

        private static UsuarioResponseDTO MapToResponseDTO(Usuario usuario)
        {
            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Idade = usuario.Idade,
                NivelRisco = usuario.NivelRisco,
                TotalApostas = usuario.TotalApostas,
                ValorGasto = usuario.ValorGasto,
                Alertas = usuario.Alertas.Select(a => new AlertaResponseDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    NomeUsuario = usuario.Nome,
                    Mensagem = a.Mensagem,
                    Tipo = a.Tipo,
                    DataCriacao = a.DataCriacao
                }).ToList()
            };
        }
    }
}
