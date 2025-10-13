using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetGuardianAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de usuários do sistema BetGuardian
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Obtém todos os usuários cadastrados
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Retorna a lista de usuários</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuarios()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtém um usuário específico pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Retorna o usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioResponseDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UsuarioResponseDTO>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
                return NotFound($"Usuário com ID {id} não encontrado.");

            return Ok(usuario);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="createDto">Dados do usuário a ser criado</param>
        /// <returns>Usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UsuarioResponseDTO>> CreateUsuario(CreateUsuarioDTO createDto)
        {
            try
            {
                var usuario = await _usuarioService.CreateUsuarioAsync(createDto);
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="updateDto">Dados atualizados do usuário</param>
        /// <returns>Usuário atualizado</returns>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UsuarioResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UsuarioResponseDTO>> UpdateUsuario(int id, UpdateUsuarioDTO updateDto)
        {
            try
            {
                var usuario = await _usuarioService.UpdateUsuarioAsync(id, updateDto);
                if (usuario == null)
                    return NotFound($"Usuário com ID {id} não encontrado.");

                return Ok(usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Usuário removido com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var result = await _usuarioService.DeleteUsuarioAsync(id);
            if (!result)
                return NotFound($"Usuário com ID {id} não encontrado.");

            return NoContent();
        }

        /// <summary>
        /// Analisa o nível de risco de um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Análise de risco do usuário</returns>
        /// <response code="200">Análise realizada com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id}/analise-risco")]
        [ProducesResponseType(typeof(AnaliseRiscoDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AnaliseRiscoDTO>> AnalisarRiscoUsuario(int id)
        {
            try
            {
                var analise = await _usuarioService.AnalisarRiscoUsuarioAsync(id);
                return Ok(analise);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Obtém usuários com maior nível de risco
        /// </summary>
        /// <param name="limit">Número máximo de usuários a retornar (padrão: 10)</param>
        /// <returns>Lista de usuários com maior risco</returns>
        /// <response code="200">Lista de usuários retornada</response>
        [HttpGet("maior-risco")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuariosMaiorRisco(int limit = 10)
        {
            var usuarios = await _usuarioService.GetUsuariosMaiorRiscoAsync(limit);
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtém usuários por nível de risco
        /// </summary>
        /// <param name="nivelRisco">Nível de risco (Baixo, Medio, Alto, Critico)</param>
        /// <returns>Lista de usuários com o nível de risco especificado</returns>
        /// <response code="200">Lista de usuários retornada</response>
        [HttpGet("por-nivel-risco/{nivelRisco}")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDTO>), 200)]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuariosPorNivelRisco(NivelRisco nivelRisco)
        {
            var usuarios = await _usuarioService.GetUsuariosPorNivelRiscoAsync(nivelRisco);
            return Ok(usuarios);
        }
    }
}
