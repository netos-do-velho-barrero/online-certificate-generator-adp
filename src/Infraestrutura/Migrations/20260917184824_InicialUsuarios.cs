using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeradorCertificadosOnline.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class InicialUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "papeis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_papeis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "perfis_usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataCriacaoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacaoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SecurityStamp = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_dominio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataCriacaoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacaoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_dominio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "papeis_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClaimValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_papeis_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_papeis_claims_papeis_RoleId",
                        column: x => x.RoleId,
                        principalTable: "papeis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClaimValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_claims_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_usuarios_logins_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_papeis",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_papeis", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_usuarios_papeis_papeis_RoleId",
                        column: x => x.RoleId,
                        principalTable: "papeis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_papeis_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_tokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_usuarios_tokens_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "papeis",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("01a058f4-a048-79a3-b1a6-0f01d629a126"), "01a058f7-9492-73bc-8e4b-934c53594ed6", "Cliente", "CLIENTE" },
                    { new Guid("01a06851-5e71-7ae2-822d-21e2fadcffa4"), "01a06852-c767-7d97-84e4-6b5f0775f3e5", "Estabelecimento", "ESTABELECIMENTO" }
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "papeis",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_papeis_claims_RoleId",
                table: "papeis_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_usuarios_UsuarioId",
                table: "perfis_usuarios",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuarios",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_claims_UserId",
                table: "usuarios_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_dominio_Email",
                table: "usuarios_dominio",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_logins_UserId",
                table: "usuarios_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_papeis_RoleId",
                table: "usuarios_papeis",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "papeis_claims");

            migrationBuilder.DropTable(
                name: "perfis_usuarios");

            migrationBuilder.DropTable(
                name: "usuarios_claims");

            migrationBuilder.DropTable(
                name: "usuarios_dominio");

            migrationBuilder.DropTable(
                name: "usuarios_logins");

            migrationBuilder.DropTable(
                name: "usuarios_papeis");

            migrationBuilder.DropTable(
                name: "usuarios_tokens");

            migrationBuilder.DropTable(
                name: "papeis");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
