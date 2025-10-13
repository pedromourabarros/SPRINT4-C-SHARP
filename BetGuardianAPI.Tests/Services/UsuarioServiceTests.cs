using BetGuardianAPI.Data;
using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Repositories;
using BetGuardianAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BetGuardianAPI.Tests.Services
{
    /// <summary>
    /// Testes unitários para o serviço de usuários
    /// </summary>
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IAlertaRepository> _mockAlertaRepository;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockAlertaRepository = new Mock<IAlertaRepository>();
            _usuarioService = new UsuarioService(_mockUsuarioRepository.Object, _mockAlertaRepository.Object);
        }

        [Fact]
        public async Task AnalisarRiscoUsuario_UsuarioComRiscoAlto_DeveRetornarAnaliseCorreta()
        {
            // Arrange
            var usuarioId = 1;
            var usuario = new Usuario
            {
                Id = usuarioId,
                Nome = "João Silva",
                Email = "joao@email.com",
                Idade = 30,
                NivelRisco = NivelRisco.Medio,
                TotalApostas = 60,
                ValorGasto = 2500.00m
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _usuarioService.AnalisarRiscoUsuarioAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuarioId, resultado.UsuarioId);
            Assert.Equal("João Silva", resultado.NomeUsuario);
            Assert.Equal(NivelRisco.Alto, resultado.NivelRiscoCalculado);
            Assert.True(resultado.AlertasGerados.Count > 0);
            Assert.Contains("Nível de risco alto detectado", resultado.AlertasGerados.First());
        }

        [Fact]
        public async Task AnalisarRiscoUsuario_UsuarioComRiscoCritico_DeveRetornarAnaliseCritica()
        {
            // Arrange
            var usuarioId = 2;
            var usuario = new Usuario
            {
                Id = usuarioId,
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Idade = 35,
                NivelRisco = NivelRisco.Alto,
                TotalApostas = 120,
                ValorGasto = 8000.00m
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _usuarioService.AnalisarRiscoUsuarioAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(NivelRisco.Critico, resultado.NivelRiscoCalculado);
            Assert.Contains("Nível de risco alto detectado", resultado.AlertasGerados.First());
            Assert.True(resultado.AlertasGerados.Any(a => a.Contains("Valor gasto em apostas muito alto")));
        }

        [Fact]
        public async Task AnalisarRiscoUsuario_UsuarioNaoEncontrado_DeveLancarExcecao()
        {
            // Arrange
            var usuarioId = 999;
            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(usuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _usuarioService.AnalisarRiscoUsuarioAsync(usuarioId));
        }

        [Fact]
        public async Task GetUsuariosMaiorRisco_DeveRetornarUsuariosOrdenadosPorRisco()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "João", NivelRisco = NivelRisco.Baixo, ValorGasto = 100 },
                new Usuario { Id = 2, Nome = "Maria", NivelRisco = NivelRisco.Critico, ValorGasto = 5000 },
                new Usuario { Id = 3, Nome = "Pedro", NivelRisco = NivelRisco.Alto, ValorGasto = 2000 }
            };

            _mockUsuarioRepository.Setup(x => x.GetUsuariosMaiorRiscoAsync(10))
                .ReturnsAsync(usuarios.OrderByDescending(u => u.NivelRisco).ThenByDescending(u => u.ValorGasto));

            // Act
            var resultado = await _usuarioService.GetUsuariosMaiorRiscoAsync(10);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            // O primeiro usuário deve ser o de maior risco (Maria - Crítico)
            Assert.Equal("Maria", resultado.First().Nome);
        }
    }
}
