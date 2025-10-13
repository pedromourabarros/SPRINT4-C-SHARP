using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetGuardianAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de alertas do sistema BetGuardian
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaService _alertaService;

        public AlertasController(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        /// <summary>
        /// Obtém todos os alertas cadastrados
        /// </summary>
        /// <returns>Lista de alertas</returns>
        /// <response code="200">Retorna a lista de alertas</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AlertaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AlertaResponseDTO>>> GetAlertas()
        {
            var alertas = await _alertaService.GetAllAlertasAsync();
            return Ok(alertas);
        }

        /// <summary>
        /// Obtém um alerta específico pelo ID
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Dados do alerta</returns>
        /// <response code="200">Retorna o alerta encontrado</response>
        /// <response code="404">Alerta não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlertaResponseDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AlertaResponseDTO>> GetAlerta(int id)
        {
            var alerta = await _alertaService.GetAlertaByIdAsync(id);
            if (alerta == null)
                return NotFound($"Alerta com ID {id} não encontrado.");

            return Ok(alerta);
        }

        /// <summary>
        /// Cria um novo alerta
        /// </summary>
        /// <param name="createDto">Dados do alerta a ser criado</param>
        /// <returns>Alerta criado</returns>
        /// <response code="201">Alerta criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(AlertaResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AlertaResponseDTO>> CreateAlerta(CreateAlertaDTO createDto)
        {
            try
            {
                var alerta = await _alertaService.CreateAlertaAsync(createDto);
                return CreatedAtAction(nameof(GetAlerta), new { id = alerta.Id }, alerta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um alerta existente
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <param name="updateDto">Dados atualizados do alerta</param>
        /// <returns>Alerta atualizado</returns>
        /// <response code="200">Alerta atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Alerta não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AlertaResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AlertaResponseDTO>> UpdateAlerta(int id, UpdateAlertaDTO updateDto)
        {
            var alerta = await _alertaService.UpdateAlertaAsync(id, updateDto);
            if (alerta == null)
                return NotFound($"Alerta com ID {id} não encontrado.");

            return Ok(alerta);
        }

        /// <summary>
        /// Remove um alerta
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Alerta removido com sucesso</response>
        /// <response code="404">Alerta não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAlerta(int id)
        {
            var result = await _alertaService.DeleteAlertaAsync(id);
            if (!result)
                return NotFound($"Alerta com ID {id} não encontrado.");

            return NoContent();
        }

        /// <summary>
        /// Obtém alertas de um usuário específico
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Lista de alertas do usuário</returns>
        /// <response code="200">Lista de alertas retornada</response>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<AlertaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AlertaResponseDTO>>> GetAlertasByUsuario(int usuarioId)
        {
            var alertas = await _alertaService.GetAlertasByUsuarioIdAsync(usuarioId);
            return Ok(alertas);
        }

        /// <summary>
        /// Obtém alertas por tipo
        /// </summary>
        /// <param name="tipo">Tipo do alerta (Informativo, Aviso, Critico, Motivacional)</param>
        /// <returns>Lista de alertas do tipo especificado</returns>
        /// <response code="200">Lista de alertas retornada</response>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(IEnumerable<AlertaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AlertaResponseDTO>>> GetAlertasByTipo(TipoAlerta tipo)
        {
            var alertas = await _alertaService.GetAlertasByTipoAsync(tipo);
            return Ok(alertas);
        }

        /// <summary>
        /// Obtém alertas recentes
        /// </summary>
        /// <param name="dias">Número de dias para buscar alertas (padrão: 7)</param>
        /// <returns>Lista de alertas recentes</returns>
        /// <response code="200">Lista de alertas retornada</response>
        [HttpGet("recentes")]
        [ProducesResponseType(typeof(IEnumerable<AlertaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AlertaResponseDTO>>> GetAlertasRecentes(int dias = 7)
        {
            var alertas = await _alertaService.GetAlertasRecentesAsync(dias);
            return Ok(alertas);
        }
    }
}
