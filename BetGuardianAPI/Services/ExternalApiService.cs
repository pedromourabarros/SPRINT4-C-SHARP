using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using Newtonsoft.Json;
using System.Text;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Implementação do serviço para integração com APIs externas
    /// </summary>
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAlertaService _alertaService;

        public ExternalApiService(HttpClient httpClient, IAlertaService alertaService)
        {
            _httpClient = httpClient;
            _alertaService = alertaService;
        }

        public async Task<string> GetMensagemMotivacionalAsync()
        {
            try
            {
                // Simulando uma API de mensagens motivacionais
                // Em um cenário real, você usaria uma API como quotable.io ou similar
                var mensagens = new[]
                {
                    "A vida é 10% do que acontece com você e 90% de como você reage a isso.",
                    "O sucesso não é final, o fracasso não é fatal: é a coragem de continuar que conta.",
                    "A única maneira de fazer um excelente trabalho é amar o que você faz.",
                    "Seja a mudança que você quer ver no mundo.",
                    "Acredite em si mesmo e tudo se tornará possível.",
                    "Cada dia é uma nova oportunidade de ser melhor que ontem.",
                    "A persistência é o caminho do êxito.",
                    "Sonhe grande e comece pequeno, mas comece agora."
                };

                var random = new Random();
                return mensagens[random.Next(mensagens.Length)];
            }
            catch (Exception ex)
            {
                return $"Mensagem motivacional: Continue focado em seus objetivos! (Erro na API: {ex.Message})";
            }
        }

        public async Task<string> GetInformacaoClimaAsync(string cidade = "São Paulo")
        {
            try
            {
                // Usando a API Open-Meteo (gratuita e sem necessidade de chave)
                var url = $"https://api.open-meteo.com/v1/forecast?latitude=-23.5475&longitude=-46.6361&current_weather=true&timezone=America/Sao_Paulo";
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<dynamic>(content);
                    
                    var temperatura = weatherData?.current_weather?.temperature;
                    var descricao = weatherData?.current_weather?.weathercode;
                    
                    return $"Clima em {cidade}: {temperatura}°C. Um bom dia para atividades ao ar livre!";
                }
                else
                {
                    return $"Clima em {cidade}: Informação não disponível no momento. Aproveite o dia!";
                }
            }
            catch (Exception ex)
            {
                return $"Clima em {cidade}: Informação não disponível. (Erro: {ex.Message})";
            }
        }

        public async Task<string> GetNoticiaSaudeMentalAsync()
        {
            try
            {
                // Simulando uma API de notícias sobre saúde mental
                // Em um cenário real, você usaria NewsAPI ou similar
                var noticias = new[]
                {
                    "Estudos mostram que atividades físicas regulares reduzem significativamente o estresse e a ansiedade.",
                    "Meditação e mindfulness são técnicas comprovadas para melhorar o bem-estar mental.",
                    "Manter conexões sociais positivas é fundamental para a saúde mental.",
                    "Terapia e apoio profissional são recursos valiosos para lidar com desafios emocionais.",
                    "Estabelecer rotinas saudáveis contribui para o equilíbrio mental e emocional.",
                    "A prática de hobbies e atividades criativas melhora a qualidade de vida.",
                    "Dormir bem é essencial para a saúde mental e o bem-estar geral.",
                    "Buscar ajuda quando necessário é um sinal de força e autocuidado."
                };

                var random = new Random();
                return noticias[random.Next(noticias.Length)];
            }
            catch (Exception ex)
            {
                return "Dica de saúde mental: Cuide-se com carinho e busque apoio quando necessário.";
            }
        }

        public async Task<AlertaResponseDTO> CriarAlertaComApiExternaAsync(int usuarioId, string tipoApi)
        {
            string mensagem;
            TipoAlerta tipoAlerta;

            switch (tipoApi.ToLower())
            {
                case "motivacional":
                    mensagem = await GetMensagemMotivacionalAsync();
                    tipoAlerta = TipoAlerta.Motivacional;
                    break;
                case "clima":
                    mensagem = await GetInformacaoClimaAsync();
                    tipoAlerta = TipoAlerta.Informativo;
                    break;
                case "saude":
                    mensagem = await GetNoticiaSaudeMentalAsync();
                    tipoAlerta = TipoAlerta.Informativo;
                    break;
                default:
                    mensagem = "Informação externa não disponível no momento.";
                    tipoAlerta = TipoAlerta.Informativo;
                    break;
            }

            return await _alertaService.CriarAlertaMotivacionalAsync(usuarioId, mensagem);
        }
    }
}
