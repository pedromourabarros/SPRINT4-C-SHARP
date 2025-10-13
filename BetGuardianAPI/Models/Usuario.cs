using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetGuardianAPI.Models
{
    /// <summary>
    /// Modelo que representa um usuário do sistema BetGuardian
    /// </summary>
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

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
        public NivelRisco NivelRisco { get; set; } = NivelRisco.Baixo;

        [Required]
        [Range(0, int.MaxValue)]
        public int TotalApostas { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal ValorGasto { get; set; } = 0;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        // Propriedades de navegação
        public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }

    /// <summary>
    /// Enum que define os níveis de risco do usuário
    /// </summary>
    public enum NivelRisco
    {
        Baixo = 1,
        Medio = 2,
        Alto = 3,
        Critico = 4
    }
}
