using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petiatra.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tutor",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    cpf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 11, nullable: false),
                    telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tutor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veterinario",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    crmv = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    senha_hash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    criado_em = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_veterinario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pet",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    especie = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    raca = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    sexo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    data_nascimento = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    peso_kg = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    tutor_id = table.Column<int>(type: "INTEGER", nullable: false),
                    criado_em = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet", x => x.id);
                    table.ForeignKey(
                        name: "fk_pet_tutor",
                        column: x => x.tutor_id,
                        principalTable: "tutor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "consulta",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    data_hora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    motivo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    diagnostico = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    tratamento = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    pet_id = table.Column<int>(type: "INTEGER", nullable: false),
                    veterinario_id = table.Column<int>(type: "INTEGER", nullable: false),
                    criado_em = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consulta", x => x.id);
                    table.ForeignKey(
                        name: "fk_consulta_pet",
                        column: x => x.pet_id,
                        principalTable: "pet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_consulta_veterinario",
                        column: x => x.veterinario_id,
                        principalTable: "veterinario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vacina",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    data_aplicacao = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    data_reforco = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    lote = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    pet_id = table.Column<int>(type: "INTEGER", nullable: false),
                    veterinario_id = table.Column<int>(type: "INTEGER", nullable: false),
                    criado_em = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vacina", x => x.id);
                    table.ForeignKey(
                        name: "fk_vacina_pet",
                        column: x => x.pet_id,
                        principalTable: "pet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vacina_veterinario",
                        column: x => x.veterinario_id,
                        principalTable: "veterinario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_consulta_data_hora",
                table: "consulta",
                column: "data_hora");

            migrationBuilder.CreateIndex(
                name: "ix_consulta_pet_id",
                table: "consulta",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_consulta_veterinario_id_data_hora",
                table: "consulta",
                columns: new[] { "veterinario_id", "data_hora" });

            migrationBuilder.CreateIndex(
                name: "ix_pet_nome",
                table: "pet",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_pet_tutor_id",
                table: "pet",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "ix_tutor_cpf",
                table: "tutor",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tutor_nome",
                table: "tutor",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_vacina_data_reforco",
                table: "vacina",
                column: "data_reforco");

            migrationBuilder.CreateIndex(
                name: "ix_vacina_pet_id",
                table: "vacina",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_vacina_veterinario_id",
                table: "vacina",
                column: "veterinario_id");

            migrationBuilder.CreateIndex(
                name: "ix_veterinario_crmv",
                table: "veterinario",
                column: "crmv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_veterinario_email",
                table: "veterinario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consulta");

            migrationBuilder.DropTable(
                name: "vacina");

            migrationBuilder.DropTable(
                name: "pet");

            migrationBuilder.DropTable(
                name: "veterinario");

            migrationBuilder.DropTable(
                name: "tutor");
        }
    }
}
