using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CadernetaObras.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaECarimboTempo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TsaAutoridade",
                table: "assinaturas_termo_conclusao",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TsaDataHora",
                table: "assinaturas_termo_conclusao",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TsaToken",
                table: "assinaturas_termo_conclusao",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TsaAutoridade",
                table: "assinaturas_relato",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TsaDataHora",
                table: "assinaturas_relato",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TsaToken",
                table: "assinaturas_relato",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TsaAutoridade",
                table: "assinaturas_obra",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TsaDataHora",
                table: "assinaturas_obra",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TsaToken",
                table: "assinaturas_obra",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "logs_auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataHoraUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Acao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntidadeTipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EntidadeId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Detalhes = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_auditoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_DataHoraUtc",
                table: "logs_auditoria",
                column: "DataHoraUtc");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_UsuarioId",
                table: "logs_auditoria",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs_auditoria");

            migrationBuilder.DropColumn(
                name: "TsaAutoridade",
                table: "assinaturas_termo_conclusao");

            migrationBuilder.DropColumn(
                name: "TsaDataHora",
                table: "assinaturas_termo_conclusao");

            migrationBuilder.DropColumn(
                name: "TsaToken",
                table: "assinaturas_termo_conclusao");

            migrationBuilder.DropColumn(
                name: "TsaAutoridade",
                table: "assinaturas_relato");

            migrationBuilder.DropColumn(
                name: "TsaDataHora",
                table: "assinaturas_relato");

            migrationBuilder.DropColumn(
                name: "TsaToken",
                table: "assinaturas_relato");

            migrationBuilder.DropColumn(
                name: "TsaAutoridade",
                table: "assinaturas_obra");

            migrationBuilder.DropColumn(
                name: "TsaDataHora",
                table: "assinaturas_obra");

            migrationBuilder.DropColumn(
                name: "TsaToken",
                table: "assinaturas_obra");
        }
    }
}
