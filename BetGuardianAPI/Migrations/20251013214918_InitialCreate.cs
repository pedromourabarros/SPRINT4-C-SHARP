using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BetGuardianAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtividadesAlternativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NivelDificuldade = table.Column<int>(type: "INTEGER", nullable: false),
                    TempoEstimadoMinutos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadesAlternativas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Idade = table.Column<int>(type: "INTEGER", nullable: false),
                    NivelRisco = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalApostas = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorGasto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AtividadesAlternativas",
                columns: new[] { "Id", "Categoria", "Descricao", "NivelDificuldade", "Nome", "TempoEstimadoMinutos" },
                values: new object[,]
                {
                    { 1, "Bem-estar", "Pratique meditação por 10-15 minutos para reduzir o estresse e ansiedade", 2, "Meditação", 15 },
                    { 2, "Fitness", "Faça uma caminhada, corrida ou exercícios em casa para liberar endorfinas", 3, "Exercícios Físicos", 30 },
                    { 3, "Educação", "Leia um livro interessante para distrair a mente e expandir conhecimentos", 1, "Leitura", 45 },
                    { 4, "Arte", "Pratique desenho, pintura, música ou qualquer atividade criativa", 2, "Hobby Criativo", 60 },
                    { 5, "Social", "Jogue xadrez, damas ou outros jogos de estratégia com amigos ou família", 2, "Jogos de Tabuleiro", 90 },
                    { 6, "Culinária", "Experimente uma nova receita ou prepare uma refeição especial", 3, "Cozinhar", 60 },
                    { 7, "Natureza", "Cuide de plantas, plante sementes ou organize um jardim", 2, "Jardinagem", 45 },
                    { 8, "Social", "Participe de atividades voluntárias na comunidade", 4, "Voluntariado", 120 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_DataCriacao",
                table: "Alertas",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_UsuarioId",
                table: "Alertas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadesAlternativas_Categoria",
                table: "AtividadesAlternativas",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NivelRisco",
                table: "Usuarios",
                column: "NivelRisco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AtividadesAlternativas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
