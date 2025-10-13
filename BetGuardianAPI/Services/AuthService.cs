using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Serviço responsável pela autenticação e geração de tokens JWT
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Verifica se o email já existe
            var existingUser = await _usuarioRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Já existe um usuário com este email.");
            }

            // Criptografa a senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Senha);

            // Cria o usuário
            var usuario = new Usuario
            {
                Nome = registerDto.Nome,
                Email = registerDto.Email,
                Idade = registerDto.Idade,
                PasswordHash = passwordHash,
                TotalApostas = 0,
                ValorGasto = 0,
                NivelRisco = NivelRisco.Baixo
            };

            var createdUser = await _usuarioRepository.CreateAsync(usuario);

            // Gera o token JWT
            var token = GenerateJwtToken(createdUser.Id, createdUser.Email);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new AuthResponseDTO
            {
                Token = token,
                Nome = createdUser.Nome,
                Email = createdUser.Email,
                Id = createdUser.Id,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            // Busca o usuário pelo email
            var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);
            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            // Verifica a senha
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            // Gera o token JWT
            var token = GenerateJwtToken(usuario.Id, usuario.Email);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new AuthResponseDTO
            {
                Token = token,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Id = usuario.Id,
                ExpiresAt = expiresAt
            };
        }

        public async Task<bool> ValidateUserAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(senha, usuario.PasswordHash);
        }

        public string GenerateJwtToken(int userId, string email)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "BetGuardian_SecretKey_2024_Sprint4_FIAP";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "BetGuardianAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "BetGuardianUsers";

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
