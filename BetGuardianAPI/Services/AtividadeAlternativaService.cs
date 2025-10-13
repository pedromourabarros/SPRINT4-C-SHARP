using BetGuardianAPI.DTOs;
using BetGuardianAPI.Models;
using BetGuardianAPI.Repositories;

namespace BetGuardianAPI.Services
{
    /// <summary>
    /// Implementação do serviço de atividades alternativas
    /// </summary>
    public class AtividadeAlternativaService : IAtividadeAlternativaService
    {
        private readonly IAtividadeAlternativaRepository _atividadeRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AtividadeAlternativaService(IAtividadeAlternativaRepository atividadeRepository, IUsuarioRepository usuarioRepository)
        {
            _atividadeRepository = atividadeRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAllAtividadesAsync()
        {
            var atividades = await _atividadeRepository.GetAllAsync();
            return atividades.Select(MapToResponseDTO);
        }

        public async Task<AtividadeAlternativaResponseDTO?> GetAtividadeByIdAsync(int id)
        {
            var atividade = await _atividadeRepository.GetByIdAsync(id);
            return atividade != null ? MapToResponseDTO(atividade) : null;
        }

        public async Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAtividadesByCategoriaAsync(string categoria)
        {
            var atividades = await _atividadeRepository.GetByCategoriaAsync(categoria);
            return atividades.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<AtividadeAlternativaResponseDTO>> GetAtividadesByNivelDificuldadeAsync(int nivelDificuldade)
        {
            var atividades = await _atividadeRepository.GetByNivelDificuldadeAsync(nivelDificuldade);
            return atividades.Select(MapToResponseDTO);
        }

        public async Task<AtividadeAlternativaResponseDTO> CreateAtividadeAsync(CreateAtividadeAlternativaDTO createDto)
        {
            var atividade = new AtividadeAlternativa
            {
                Nome = createDto.Nome,
                Descricao = createDto.Descricao,
                Categoria = createDto.Categoria,
                NivelDificuldade = createDto.NivelDificuldade,
                TempoEstimadoMinutos = createDto.TempoEstimadoMinutos
            };

            var createdAtividade = await _atividadeRepository.CreateAsync(atividade);
            return MapToResponseDTO(createdAtividade);
        }

        public async Task<AtividadeAlternativaResponseDTO?> UpdateAtividadeAsync(int id, UpdateAtividadeAlternativaDTO updateDto)
        {
            var atividade = await _atividadeRepository.GetByIdAsync(id);
            if (atividade == null)
                return null;

            if (!string.IsNullOrEmpty(updateDto.Nome))
                atividade.Nome = updateDto.Nome;
            if (!string.IsNullOrEmpty(updateDto.Descricao))
                atividade.Descricao = updateDto.Descricao;
            if (!string.IsNullOrEmpty(updateDto.Categoria))
                atividade.Categoria = updateDto.Categoria;
            if (updateDto.NivelDificuldade.HasValue)
                atividade.NivelDificuldade = updateDto.NivelDificuldade.Value;
            if (updateDto.TempoEstimadoMinutos.HasValue)
                atividade.TempoEstimadoMinutos = updateDto.TempoEstimadoMinutos.Value;

            var updatedAtividade = await _atividadeRepository.UpdateAsync(atividade);
            return MapToResponseDTO(updatedAtividade);
        }

        public async Task<bool> DeleteAtividadeAsync(int id)
        {
            return await _atividadeRepository.DeleteAsync(id);
        }

        public async Task<SugestaoAtividadeDTO> GetSugestaoAtividadesParaUsuarioAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            var atividadesSugeridas = await _atividadeRepository.GetAtividadesSugeridasAsync(usuario.NivelRisco);
            var mensagemMotivacional = GerarMensagemMotivacional(usuario.NivelRisco);

            return new SugestaoAtividadeDTO
            {
                UsuarioId = usuario.Id,
                NomeUsuario = usuario.Nome,
                NivelRisco = usuario.NivelRisco,
                AtividadesSugeridas = atividadesSugeridas.Select(MapToResponseDTO).ToList(),
                MensagemMotivacional = mensagemMotivacional
            };
        }

        private static string GerarMensagemMotivacional(NivelRisco nivelRisco)
        {
            var mensagens = nivelRisco switch
            {
                NivelRisco.Baixo => new[]
                {
                    "Continue assim! Você está no controle das suas atividades.",
                    "Parabéns por manter um equilíbrio saudável!",
                    "Você está fazendo um excelente trabalho mantendo-se focado."
                },
                NivelRisco.Medio => new[]
                {
                    "Você está no caminho certo! Considere explorar novas atividades.",
                    "Cada passo em direção ao equilíbrio é uma vitória!",
                    "Você tem o poder de fazer escolhas positivas para sua vida."
                },
                NivelRisco.Alto => new[]
                {
                    "Você é mais forte do que imagina. Busque apoio e atividades que te tragam paz.",
                    "Cada dia é uma nova oportunidade de fazer escolhas melhores.",
                    "Lembre-se: pedir ajuda é um sinal de força, não de fraqueza."
                },
                NivelRisco.Critico => new[]
                {
                    "Você não está sozinho. Existem pessoas e recursos prontos para te ajudar.",
                    "A recuperação é possível. Acredite em si mesmo e busque apoio profissional.",
                    "Cada momento é uma chance de recomeçar. Você merece uma vida plena e feliz."
                },
                _ => new[] { "Continue focado em suas metas e objetivos." }
            };

            var random = new Random();
            return mensagens[random.Next(mensagens.Length)];
        }

        private static AtividadeAlternativaResponseDTO MapToResponseDTO(AtividadeAlternativa atividade)
        {
            return new AtividadeAlternativaResponseDTO
            {
                Id = atividade.Id,
                Nome = atividade.Nome,
                Descricao = atividade.Descricao,
                Categoria = atividade.Categoria,
                NivelDificuldade = atividade.NivelDificuldade,
                TempoEstimadoMinutos = atividade.TempoEstimadoMinutos
            };
        }
    }
}
