using BetGuardianAPI.Data;
using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace BetGuardianAPI.Tests.Integration
{
    /// <summary>
    /// Testes de integração para a API completa
    /// </summary>
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o contexto de banco real
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<BetGuardianContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Adiciona contexto em memória para testes
                    services.AddDbContext<BetGuardianContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_NovoUsuario_DeveRetornarToken()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Nome = "Teste Usuario",
                Email = "teste@email.com",
                Idade = 25,
                Senha = "123456"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
            Assert.NotNull(authResponse);
            Assert.NotEmpty(authResponse.Token);
            Assert.Equal(registerDto.Nome, authResponse.Nome);
        }

        [Fact]
        public async Task Login_UsuarioExistente_DeveRetornarToken()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "joao.silva@email.com",
                Senha = "123456"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
            Assert.NotNull(authResponse);
            Assert.NotEmpty(authResponse.Token);
        }

        [Fact]
        public async Task GetUsuarios_SemToken_DeveRetornarUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/usuarios");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUsuarios_ComTokenValido_DeveRetornarUsuarios()
        {
            // Arrange - Primeiro fazer login para obter token
            var loginDto = new LoginDTO
            {
                Email = "joao.silva@email.com",
                Senha = "123456"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDTO>();

            // Configurar header de autorização
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

            // Act
            var response = await _client.GetAsync("/api/usuarios");

            // Assert
            response.EnsureSuccessStatusCode();
            var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<UsuarioResponseDTO>>();
            Assert.NotNull(usuarios);
        }

        [Fact]
        public async Task GetAtividadesAlternativas_ComTokenValido_DeveRetornarAtividades()
        {
            // Arrange - Primeiro fazer login para obter token
            var loginDto = new LoginDTO
            {
                Email = "joao.silva@email.com",
                Senha = "123456"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDTO>();

            // Configurar header de autorização
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.Token);

            // Act
            var response = await _client.GetAsync("/api/atividadesalternativas");

            // Assert
            response.EnsureSuccessStatusCode();
            var atividades = await response.Content.ReadFromJsonAsync<IEnumerable<AtividadeAlternativaResponseDTO>>();
            Assert.NotNull(atividades);
            Assert.True(atividades!.Count() > 0);
        }
    }
}
