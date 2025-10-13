using BetGuardianAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace BetGuardianAPI.DTOs
{
    /// <summary>
    /// DTO para criação de alerta
    /// </summary>
    public class CreateAlertaDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(500)]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        public TipoAlerta Tipo { get; set; }
    }

    /// <summary>
    /// DTO para atualização de alerta
    /// </summary>
    public class UpdateAlertaDTO
    {
        [StringLength(500)]
        public string? Mensagem { get; set; }

        public TipoAlerta? Tipo { get; set; }
    }

    /// <summary>
    /// DTO para resposta de alerta
    /// </summary>
    public class AlertaResponseDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public TipoAlerta Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
