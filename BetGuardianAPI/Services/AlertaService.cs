using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Repositories;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Implementação do serviço de alertas
    /// </summary>
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _alertaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AlertaService(IAlertaRepository alertaRepository, IUsuarioRepository usuarioRepository)
        {
            _alertaRepository = alertaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<AlertaResponseDTO>> GetAllAlertasAsync()
        {
            var alertas = await _alertaRepository.GetAllAsync();
            return alertas.Select(MapToResponseDTO);
        }

        public async Task<AlertaResponseDTO?> GetAlertaByIdAsync(int id)
        {
            var alerta = await _alertaRepository.GetByIdAsync(id);
            return alerta != null ? MapToResponseDTO(alerta) : null;
        }

        public async Task<IEnumerable<AlertaResponseDTO>> GetAlertasByUsuarioIdAsync(int usuarioId)
        {
            var alertas = await _alertaRepository.GetByUsuarioIdAsync(usuarioId);
            return alertas.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<AlertaResponseDTO>> GetAlertasByTipoAsync(TipoAlerta tipo)
        {
            var alertas = await _alertaRepository.GetByTipoAsync(tipo);
            return alertas.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<AlertaResponseDTO>> GetAlertasRecentesAsync(int dias = 7)
        {
            var alertas = await _alertaRepository.GetAlertasRecentesAsync(dias);
            return alertas.Select(MapToResponseDTO);
        }

        public async Task<AlertaResponseDTO> CreateAlertaAsync(CreateAlertaDTO createDto)
        {
            // Verificar se o usuário existe
            var usuario = await _usuarioRepository.GetByIdAsync(createDto.UsuarioId);
            if (usuario == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            var alerta = new Alerta
            {
                UsuarioId = createDto.UsuarioId,
                Mensagem = createDto.Mensagem,
                Tipo = createDto.Tipo,
                DataCriacao = DateTime.UtcNow
            };

            var createdAlerta = await _alertaRepository.CreateAsync(alerta);
            return MapToResponseDTO(createdAlerta);
        }

        public async Task<AlertaResponseDTO?> UpdateAlertaAsync(int id, UpdateAlertaDTO updateDto)
        {
            var alerta = await _alertaRepository.GetByIdAsync(id);
            if (alerta == null)
                return null;

            if (!string.IsNullOrEmpty(updateDto.Mensagem))
                alerta.Mensagem = updateDto.Mensagem;
            if (updateDto.Tipo.HasValue)
                alerta.Tipo = updateDto.Tipo.Value;

            var updatedAlerta = await _alertaRepository.UpdateAsync(alerta);
            return MapToResponseDTO(updatedAlerta);
        }

        public async Task<bool> DeleteAlertaAsync(int id)
        {
            return await _alertaRepository.DeleteAsync(id);
        }

        public async Task<AlertaResponseDTO> CriarAlertaMotivacionalAsync(int usuarioId, string mensagem)
        {
            var createDto = new CreateAlertaDTO
            {
                UsuarioId = usuarioId,
                Mensagem = mensagem,
                Tipo = TipoAlerta.Motivacional
            };

            return await CreateAlertaAsync(createDto);
        }

        private static AlertaResponseDTO MapToResponseDTO(Alerta alerta)
        {
            return new AlertaResponseDTO
            {
                Id = alerta.Id,
                UsuarioId = alerta.UsuarioId,
                NomeUsuario = alerta.Usuario?.Nome ?? "Usuário não encontrado",
                Mensagem = alerta.Mensagem,
                Tipo = alerta.Tipo,
                DataCriacao = alerta.DataCriacao
            };
        }
    }
}
