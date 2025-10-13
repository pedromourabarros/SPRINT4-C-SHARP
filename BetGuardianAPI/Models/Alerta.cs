using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetGuardianAPI.Models
{
    /// <summary>
    /// Modelo que representa um alerta para o usuário
    /// </summary>
    public class Alerta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(500)]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        public TipoAlerta Tipo { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Propriedade de navegação
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }

    /// <summary>
    /// Enum que define os tipos de alerta
    /// </summary>
    public enum TipoAlerta
    {
        Informativo = 1,
        Aviso = 2,
        Critico = 3,
        Motivacional = 4
    }
}
