using BetGuardianAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace BetGuardianAPI.DTOs
{
    /// <summary>
    /// DTO para criação de usuário (apenas para administradores)
    /// </summary>
    public class CreateUsuarioDTO
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int Idade { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int TotalApostas { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal ValorGasto { get; set; } = 0;
    }

    /// <summary>
    /// DTO para atualização de usuário
    /// </summary>
    public class UpdateUsuarioDTO
    {
        [StringLength(100)]
        public string? Nome { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int? Idade { get; set; }

        [Range(0, int.MaxValue)]
        public int? TotalApostas { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ValorGasto { get; set; }
    }

    /// <summary>
    /// DTO para resposta de usuário
    /// </summary>
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Idade { get; set; }
        public NivelRisco NivelRisco { get; set; }
        public int TotalApostas { get; set; }
        public decimal ValorGasto { get; set; }
        public List<AlertaResponseDTO> Alertas { get; set; } = new List<AlertaResponseDTO>();
    }

    /// <summary>
    /// DTO para análise de risco do usuário
    /// </summary>
    public class AnaliseRiscoDTO
    {
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public NivelRisco NivelRiscoAtual { get; set; }
        public NivelRisco NivelRiscoCalculado { get; set; }
        public decimal ValorGasto { get; set; }
        public int TotalApostas { get; set; }
        public string Recomendacao { get; set; } = string.Empty;
        public List<string> AlertasGerados { get; set; } = new List<string>();
    }
}
