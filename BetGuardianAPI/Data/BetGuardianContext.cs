using BetGuardianAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BetGuardianAPI.Data
{
    /// <summary>
    /// Contexto do Entity Framework para o sistema BetGuardian
    /// </summary>
    public class BetGuardianContext : DbContext
    {
        public BetGuardianContext(DbContextOptions<BetGuardianContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<AtividadeAlternativa> AtividadesAlternativas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento Usuario -> Alertas
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Alertas)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração de índices para melhor performance
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NivelRisco);

            modelBuilder.Entity<Alerta>()
                .HasIndex(a => a.UsuarioId);

            modelBuilder.Entity<Alerta>()
                .HasIndex(a => a.DataCriacao);

            modelBuilder.Entity<AtividadeAlternativa>()
                .HasIndex(a => a.Categoria);

            // Seed data para atividades alternativas
            modelBuilder.Entity<AtividadeAlternativa>().HasData(
                new AtividadeAlternativa
                {
                    Id = 1,
                    Nome = "Meditação",
                    Descricao = "Pratique meditação por 10-15 minutos para reduzir o estresse e ansiedade",
                    Categoria = "Bem-estar",
                    NivelDificuldade = 2,
                    TempoEstimadoMinutos = 15
                },
                new AtividadeAlternativa
                {
                    Id = 2,
                    Nome = "Exercícios Físicos",
                    Descricao = "Faça uma caminhada, corrida ou exercícios em casa para liberar endorfinas",
                    Categoria = "Fitness",
                    NivelDificuldade = 3,
                    TempoEstimadoMinutos = 30
                },
                new AtividadeAlternativa
                {
                    Id = 3,
                    Nome = "Leitura",
                    Descricao = "Leia um livro interessante para distrair a mente e expandir conhecimentos",
                    Categoria = "Educação",
                    NivelDificuldade = 1,
                    TempoEstimadoMinutos = 45
                },
                new AtividadeAlternativa
                {
                    Id = 4,
                    Nome = "Hobby Criativo",
                    Descricao = "Pratique desenho, pintura, música ou qualquer atividade criativa",
                    Categoria = "Arte",
                    NivelDificuldade = 2,
                    TempoEstimadoMinutos = 60
                },
                new AtividadeAlternativa
                {
                    Id = 5,
                    Nome = "Jogos de Tabuleiro",
                    Descricao = "Jogue xadrez, damas ou outros jogos de estratégia com amigos ou família",
                    Categoria = "Social",
                    NivelDificuldade = 2,
                    TempoEstimadoMinutos = 90
                },
                new AtividadeAlternativa
                {
                    Id = 6,
                    Nome = "Cozinhar",
                    Descricao = "Experimente uma nova receita ou prepare uma refeição especial",
                    Categoria = "Culinária",
                    NivelDificuldade = 3,
                    TempoEstimadoMinutos = 60
                },
                new AtividadeAlternativa
                {
                    Id = 7,
                    Nome = "Jardinagem",
                    Descricao = "Cuide de plantas, plante sementes ou organize um jardim",
                    Categoria = "Natureza",
                    NivelDificuldade = 2,
                    TempoEstimadoMinutos = 45
                },
                new AtividadeAlternativa
                {
                    Id = 8,
                    Nome = "Voluntariado",
                    Descricao = "Participe de atividades voluntárias na comunidade",
                    Categoria = "Social",
                    NivelDificuldade = 4,
                    TempoEstimadoMinutos = 120
                }
            );
        }
    }
}
