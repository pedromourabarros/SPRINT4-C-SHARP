using BetGuardianAPI.DTOs;
using BetGuardianAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetGuardianAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de atividades alternativas do sistema BetGuardian
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AtividadesAlternativasController : ControllerBase
    {
        private readonly IAtividadeAlternativaService _atividadeService;

        public AtividadesAlternativasController(IAtividadeAlternativaService atividadeService)
        {
            _atividadeService = atividadeService;
        }

        /// <summary>
        /// Obtém todas as atividades alternativas cadastradas
        /// </summary>
        /// <returns>Lista de atividades alternativas</returns>
        /// <response code="200">Retorna a lista de atividades</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AtividadeAlternativaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AtividadeAlternativaResponseDTO>>> GetAtividades()
        {
            var atividades = await _atividadeService.GetAllAtividadesAsync();
            return Ok(atividades);
        }

        /// <summary>
        /// Obtém uma atividade específica pelo ID
        /// </summary>
        /// <param name="id">ID da atividade</param>
        /// <returns>Dados da atividade</returns>
        /// <response code="200">Retorna a atividade encontrada</response>
        /// <response code="404">Atividade não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AtividadeAlternativaResponseDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AtividadeAlternativaResponseDTO>> GetAtividade(int id)
        {
            var atividade = await _atividadeService.GetAtividadeByIdAsync(id);
            if (atividade == null)
                return NotFound($"Atividade com ID {id} não encontrada.");

            return Ok(atividade);
        }

        /// <summary>
        /// Cria uma nova atividade alternativa
        /// </summary>
        /// <param name="createDto">Dados da atividade a ser criada</param>
        /// <returns>Atividade criada</returns>
        /// <response code="201">Atividade criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(AtividadeAlternativaResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AtividadeAlternativaResponseDTO>> CreateAtividade(CreateAtividadeAlternativaDTO createDto)
        {
            var atividade = await _atividadeService.CreateAtividadeAsync(createDto);
            return CreatedAtAction(nameof(GetAtividade), new { id = atividade.Id }, atividade);
        }

        /// <summary>
        /// Atualiza uma atividade existente
        /// </summary>
        /// <param name="id">ID da atividade</param>
        /// <param name="updateDto">Dados atualizados da atividade</param>
        /// <returns>Atividade atualizada</returns>
        /// <response code="200">Atividade atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Atividade não encontrada</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AtividadeAlternativaResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AtividadeAlternativaResponseDTO>> UpdateAtividade(int id, UpdateAtividadeAlternativaDTO updateDto)
        {
            var atividade = await _atividadeService.UpdateAtividadeAsync(id, updateDto);
            if (atividade == null)
                return NotFound($"Atividade com ID {id} não encontrada.");

            return Ok(atividade);
        }

        /// <summary>
        /// Remove uma atividade
        /// </summary>
        /// <param name="id">ID da atividade</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Atividade removida com sucesso</response>
        /// <response code="404">Atividade não encontrada</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAtividade(int id)
        {
            var result = await _atividadeService.DeleteAtividadeAsync(id);
            if (!result)
                return NotFound($"Atividade com ID {id} não encontrada.");

            return NoContent();
        }

        /// <summary>
        /// Obtém atividades por categoria
        /// </summary>
        /// <param name="categoria">Categoria das atividades</param>
        /// <returns>Lista de atividades da categoria especificada</returns>
        /// <response code="200">Lista de atividades retornada</response>
        [HttpGet("categoria/{categoria}")]
        [ProducesResponseType(typeof(IEnumerable<AtividadeAlternativaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AtividadeAlternativaResponseDTO>>> GetAtividadesByCategoria(string categoria)
        {
            var atividades = await _atividadeService.GetAtividadesByCategoriaAsync(categoria);
            return Ok(atividades);
        }

        /// <summary>
        /// Obtém atividades por nível de dificuldade
        /// </summary>
        /// <param name="nivelDificuldade">Nível de dificuldade (1-5)</param>
        /// <returns>Lista de atividades com o nível de dificuldade especificado</returns>
        /// <response code="200">Lista de atividades retornada</response>
        [HttpGet("dificuldade/{nivelDificuldade}")]
        [ProducesResponseType(typeof(IEnumerable<AtividadeAlternativaResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<AtividadeAlternativaResponseDTO>>> GetAtividadesByNivelDificuldade(int nivelDificuldade)
        {
            if (nivelDificuldade < 1 || nivelDificuldade > 5)
                return BadRequest("Nível de dificuldade deve estar entre 1 e 5.");

            var atividades = await _atividadeService.GetAtividadesByNivelDificuldadeAsync(nivelDificuldade);
            return Ok(atividades);
        }

        /// <summary>
        /// Obtém sugestões de atividades para um usuário específico
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Sugestões de atividades personalizadas</returns>
        /// <response code="200">Sugestões retornadas com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("sugestao/{usuarioId}")]
        [ProducesResponseType(typeof(SugestaoAtividadeDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SugestaoAtividadeDTO>> GetSugestaoAtividades(int usuarioId)
        {
            try
            {
                var sugestao = await _atividadeService.GetSugestaoAtividadesParaUsuarioAsync(usuarioId);
                return Ok(sugestao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
