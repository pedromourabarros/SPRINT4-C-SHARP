using BetGuardianAPI.DTOs;
using BetGuardianAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetGuardianAPI.Controllers
{
    /// <summary>
    /// Controller para integração com APIs externas do sistema BetGuardian
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ExternalApiController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;

        public ExternalApiController(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        /// <summary>
        /// Obtém uma mensagem motivacional de uma API externa
        /// </summary>
        /// <returns>Mensagem motivacional</returns>
        /// <response code="200">Mensagem retornada com sucesso</response>
        [HttpGet("mensagem-motivacional")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<string>> GetMensagemMotivacional()
        {
            var mensagem = await _externalApiService.GetMensagemMotivacionalAsync();
            return Ok(new { mensagem });
        }

        /// <summary>
        /// Obtém informações do clima de uma API externa
        /// </summary>
        /// <param name="cidade">Nome da cidade (padrão: São Paulo)</param>
        /// <returns>Informações do clima</returns>
        /// <response code="200">Informações do clima retornadas</response>
        [HttpGet("clima")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<string>> GetInformacaoClima(string cidade = "São Paulo")
        {
            var informacao = await _externalApiService.GetInformacaoClimaAsync(cidade);
            return Ok(new { informacao });
        }

        /// <summary>
        /// Obtém notícias sobre saúde mental de uma API externa
        /// </summary>
        /// <returns>Notícia sobre saúde mental</returns>
        /// <response code="200">Notícia retornada com sucesso</response>
        [HttpGet("noticia-saude-mental")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<string>> GetNoticiaSaudeMental()
        {
            var noticia = await _externalApiService.GetNoticiaSaudeMentalAsync();
            return Ok(new { noticia });
        }

        /// <summary>
        /// Cria um alerta usando dados de uma API externa
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="tipoApi">Tipo da API (motivacional, clima, saude)</param>
        /// <returns>Alerta criado com dados externos</returns>
        /// <response code="201">Alerta criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost("criar-alerta/{usuarioId}")]
        [ProducesResponseType(typeof(AlertaResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AlertaResponseDTO>> CriarAlertaComApiExterna(int usuarioId, [FromQuery] string tipoApi)
        {
            if (string.IsNullOrEmpty(tipoApi))
                return BadRequest("Tipo da API é obrigatório.");

            var tiposValidos = new[] { "motivacional", "clima", "saude" };
            if (!tiposValidos.Contains(tipoApi.ToLower()))
                return BadRequest("Tipo da API deve ser: motivacional, clima ou saude.");

            try
            {
                var alerta = await _externalApiService.CriarAlertaComApiExternaAsync(usuarioId, tipoApi);
                return CreatedAtAction(nameof(GetAlertaCriado), new { id = alerta.Id }, alerta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint auxiliar para obter um alerta criado
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Dados do alerta</returns>
        [HttpGet("alerta/{id}")]
        [ProducesResponseType(typeof(AlertaResponseDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AlertaResponseDTO>> GetAlertaCriado(int id)
        {
            // Este endpoint é usado apenas para o CreatedAtAction
            // A implementação real seria no AlertasController
            return NotFound("Use o endpoint /api/alertas/{id} para obter detalhes do alerta.");
        }
    }
}
