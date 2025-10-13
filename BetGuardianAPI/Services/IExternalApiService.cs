using BetGuardianAPI.DTOs;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Interface do serviço para integração com APIs externas
    /// </summary>
    public interface IExternalApiService
    {
        Task<string> GetMensagemMotivacionalAsync();
        Task<string> GetInformacaoClimaAsync(string cidade = "São Paulo");
        Task<string> GetNoticiaSaudeMentalAsync();
        Task<AlertaResponseDTO> CriarAlertaComApiExternaAsync(int usuarioId, string tipoApi);
    }
}
