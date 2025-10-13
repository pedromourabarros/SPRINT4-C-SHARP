using System.ComponentModel.DataAnnotations;
using BetGuardianAPI.Models;

namespace BetGuardianAPI.DTOs
{
    /// <summary>
    /// DTO para criação de atividade alternativa
    /// </summary>
    public class CreateAtividadeAlternativaDTO
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(50)]
        public string Categoria { get; set; } = "Geral";

        [Range(1, 5)]
        public int NivelDificuldade { get; set; } = 1;

        [Range(1, 300)]
        public int TempoEstimadoMinutos { get; set; } = 30;
    }

    /// <summary>
    /// DTO para atualização de atividade alternativa
    /// </summary>
    public class UpdateAtividadeAlternativaDTO
    {
        [StringLength(100)]
        public string? Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [StringLength(50)]
        public string? Categoria { get; set; }

        [Range(1, 5)]
        public int? NivelDificuldade { get; set; }

        [Range(1, 300)]
        public int? TempoEstimadoMinutos { get; set; }
    }

    /// <summary>
    /// DTO para resposta de atividade alternativa
    /// </summary>
    public class AtividadeAlternativaResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int NivelDificuldade { get; set; }
        public int TempoEstimadoMinutos { get; set; }
    }

    /// <summary>
    /// DTO para sugestão de atividades para usuário
    /// </summary>
    public class SugestaoAtividadeDTO
    {
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public NivelRisco NivelRisco { get; set; }
        public List<AtividadeAlternativaResponseDTO> AtividadesSugeridas { get; set; } = new List<AtividadeAlternativaResponseDTO>();
        public string MensagemMotivacional { get; set; } = string.Empty;
    }
}
