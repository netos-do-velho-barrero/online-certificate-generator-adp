using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificadosOnline.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateGenerationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "processamentos_certificados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CaminhoZip = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConcluidoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processamentos_certificados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "certificados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeAluno = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CaminhoPdf = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GeradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Erro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificados_processamentos_certificados_ProcessamentoId",
                        column: x => x.ProcessamentoId,
                        principalTable: "processamentos_certificados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_certificados_CursoId_UsuarioId",
                table: "certificados",
                columns: new[] { "CursoId", "UsuarioId" });

            migrationBuilder.CreateIndex(
                name: "IX_certificados_ProcessamentoId_UsuarioId",
                table: "certificados",
                columns: new[] { "ProcessamentoId", "UsuarioId" });

            migrationBuilder.CreateIndex(
                name: "ux_processamentos_ativos",
                table: "processamentos_certificados",
                columns: new[] { "CursoId", "UsuarioId" },
                unique: true,
                filter: "\"Status\" NOT IN ('Concluido', 'Falha')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificados");

            migrationBuilder.DropTable(
                name: "processamentos_certificados");
        }
    }
}
