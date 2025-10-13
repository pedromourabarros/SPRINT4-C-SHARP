using System.ComponentModel.DataAnnotations;

namespace BetGuardianAPI.Models
{
    /// <summary>
    /// Modelo que representa uma atividade alternativa para usuários com problemas de apostas
    /// </summary>
    public class AtividadeAlternativa
    {
        [Key]
        public int Id { get; set; }

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
}
