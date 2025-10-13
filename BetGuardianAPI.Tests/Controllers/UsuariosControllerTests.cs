using BetGuardianAPI.Controllers;
using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BetGuardianAPI.Tests.Controllers
{
    /// <summary>
    /// Testes de integração para o controller de usuários
    /// </summary>
    public class UsuariosControllerTests
    {
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly UsuariosController _controller;

        public UsuariosControllerTests()
        {
            _mockUsuarioService = new Mock<IUsuarioService>();
            _controller = new UsuariosController(_mockUsuarioService.Object);
            
            // Configurar contexto de usuário autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "teste@email.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [Fact]
        public async Task GetUsuarios_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            var usuarios = new List<UsuarioResponseDTO>
            {
                new UsuarioResponseDTO
                {
                    Id = 1,
                    Nome = "João Silva",
                    Email = "joao@email.com",
                    Idade = 30,
                    NivelRisco = NivelRisco.Medio,
                    TotalApostas = 25,
                    ValorGasto = 750.50m
                }
            };

            _mockUsuarioService.Setup(x => x.GetAllUsuariosAsync())
                .ReturnsAsync(usuarios);

            // Act
            var resultado = await _controller.GetUsuarios();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<UsuarioResponseDTO>>>(resultado);
            var returnValue = Assert.IsType<OkObjectResult>(okResult.Result);
            var usuariosRetornados = Assert.IsAssignableFrom<IEnumerable<UsuarioResponseDTO>>(returnValue.Value);
            Assert.Single(usuariosRetornados);
        }

        [Fact]
        public async Task GetUsuario_UsuarioExistente_DeveRetornarUsuario()
        {
            // Arrange
            var usuarioId = 1;
            var usuario = new UsuarioResponseDTO
            {
                Id = usuarioId,
                Nome = "João Silva",
                Email = "joao@email.com",
                Idade = 30,
                NivelRisco = NivelRisco.Medio,
                TotalApostas = 25,
                ValorGasto = 750.50m
            };

            _mockUsuarioService.Setup(x => x.GetUsuarioByIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _controller.GetUsuario(usuarioId);

            // Assert
            var okResult = Assert.IsType<ActionResult<UsuarioResponseDTO>>(resultado);
            var returnValue = Assert.IsType<OkObjectResult>(okResult.Result);
            var usuarioRetornado = Assert.IsType<UsuarioResponseDTO>(returnValue.Value);
            Assert.Equal(usuarioId, usuarioRetornado.Id);
        }

        [Fact]
        public async Task GetUsuario_UsuarioNaoExistente_DeveRetornarNotFound()
        {
            // Arrange
            var usuarioId = 999;
            _mockUsuarioService.Setup(x => x.GetUsuarioByIdAsync(usuarioId))
                .ReturnsAsync((UsuarioResponseDTO?)null);

            // Act
            var resultado = await _controller.GetUsuario(usuarioId);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<UsuarioResponseDTO>>(resultado);
            Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
        }

        [Fact]
        public async Task CreateUsuario_DadosValidos_DeveCriarUsuario()
        {
            // Arrange
            var createDto = new CreateUsuarioDTO
            {
                Nome = "Novo Usuario",
                Email = "novo@email.com",
                Idade = 25,
                Senha = "123456",
                TotalApostas = 0,
                ValorGasto = 0
            };

            var usuarioCriado = new UsuarioResponseDTO
            {
                Id = 1,
                Nome = createDto.Nome,
                Email = createDto.Email,
                Idade = createDto.Idade,
                NivelRisco = NivelRisco.Baixo,
                TotalApostas = createDto.TotalApostas,
                ValorGasto = createDto.ValorGasto
            };

            _mockUsuarioService.Setup(x => x.CreateUsuarioAsync(createDto))
                .ReturnsAsync(usuarioCriado);

            // Act
            var resultado = await _controller.CreateUsuario(createDto);

            // Assert
            var createdResult = Assert.IsType<ActionResult<UsuarioResponseDTO>>(resultado);
            var returnValue = Assert.IsType<CreatedAtActionResult>(createdResult.Result);
            var usuarioRetornado = Assert.IsType<UsuarioResponseDTO>(returnValue.Value);
            Assert.Equal(createDto.Nome, usuarioRetornado.Nome);
        }

        [Fact]
        public async Task AnalisarRiscoUsuario_UsuarioExistente_DeveRetornarAnalise()
        {
            // Arrange
            var usuarioId = 1;
            var analise = new AnaliseRiscoDTO
            {
                UsuarioId = usuarioId,
                NomeUsuario = "João Silva",
                NivelRiscoAtual = NivelRisco.Medio,
                NivelRiscoCalculado = NivelRisco.Alto,
                ValorGasto = 2500.00m,
                TotalApostas = 60,
                Recomendacao = "Considere reduzir a frequência das apostas",
                AlertasGerados = new List<string> { "Nível de risco alto detectado" }
            };

            _mockUsuarioService.Setup(x => x.AnalisarRiscoUsuarioAsync(usuarioId))
                .ReturnsAsync(analise);

            // Act
            var resultado = await _controller.AnalisarRiscoUsuario(usuarioId);

            // Assert
            var okResult = Assert.IsType<ActionResult<AnaliseRiscoDTO>>(resultado);
            var returnValue = Assert.IsType<OkObjectResult>(okResult.Result);
            var analiseRetornada = Assert.IsType<AnaliseRiscoDTO>(returnValue.Value);
            Assert.Equal(usuarioId, analiseRetornada.UsuarioId);
            Assert.Equal(NivelRisco.Alto, analiseRetornada.NivelRiscoCalculado);
        }
    }
}
