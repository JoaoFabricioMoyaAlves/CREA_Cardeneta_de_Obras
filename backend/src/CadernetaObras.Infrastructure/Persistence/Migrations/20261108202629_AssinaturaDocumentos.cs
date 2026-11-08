using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CadernetaObras.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AssinaturaDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "obras",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "SP");

            migrationBuilder.AddColumn<string>(
                name: "HashAssinatura",
                table: "assinaturas_termo_conclusao",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashAssinatura",
                table: "assinaturas_relato",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashAssinatura",
                table: "assinaturas_obra",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "documentos_assinaveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReferenciaId = table.Column<int>(type: "integer", nullable: false),
                    ModeloVersao = table.Column<int>(type: "integer", nullable: false),
                    DadosJson = table.Column<string>(type: "text", nullable: false),
                    HashDocumento = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_assinaveis", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documentos_assinaveis_Tipo_ReferenciaId",
                table: "documentos_assinaveis",
                columns: new[] { "Tipo", "ReferenciaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documentos_assinaveis");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "obras");

            migrationBuilder.DropColumn(
                name: "HashAssinatura",
                table: "assinaturas_termo_conclusao");

            migrationBuilder.DropColumn(
                name: "HashAssinatura",
                table: "assinaturas_relato");

            migrationBuilder.DropColumn(
                name: "HashAssinatura",
                table: "assinaturas_obra");
        }
    }
}
