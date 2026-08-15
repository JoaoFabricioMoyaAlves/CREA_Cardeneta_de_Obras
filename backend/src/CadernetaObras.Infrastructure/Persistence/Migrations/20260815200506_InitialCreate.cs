using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CadernetaObras.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroCaderneta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdAdministrador = table.Column<Guid>(type: "uuid", nullable: false),
                    IdProfissional = table.Column<Guid>(type: "uuid", nullable: false),
                    IdProprietario = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalObra = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NumeroRt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AreaConstruirM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AreaAmpliarM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AreaReformarM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AreaRegularizarM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AreaTotalEdificadaM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TipoEdificacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoEdificacaoOutros = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AtivTecnicaDirecao = table.Column<bool>(type: "boolean", nullable: false),
                    AtivTecnicaExecucao = table.Column<bool>(type: "boolean", nullable: false),
                    AtivTecnicaFiscalizacao = table.Column<bool>(type: "boolean", nullable: false),
                    AtivTecnicaProjeto = table.Column<bool>(type: "boolean", nullable: false),
                    ValorObra = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    DataReciboAbertura = table.Column<DateOnly>(type: "date", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CnpjEmpresa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TituloProfissional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NumeroRegistro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assinaturas_obra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObraId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Papel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas_obra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinaturas_obra_obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "relatos_visita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObraId = table.Column<int>(type: "integer", nullable: false),
                    DataVisita = table.Column<DateOnly>(type: "date", nullable: false),
                    PosicaoObra = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DecisoesOrientacoes = table.Column<string>(type: "text", nullable: false),
                    FaseServicosPreliminares = table.Column<bool>(type: "boolean", nullable: false),
                    FaseFundacao = table.Column<bool>(type: "boolean", nullable: false),
                    FaseAlvenarias = table.Column<bool>(type: "boolean", nullable: false),
                    FaseSuperestrutura = table.Column<bool>(type: "boolean", nullable: false),
                    FaseCobertura = table.Column<bool>(type: "boolean", nullable: false),
                    FaseEsquadriasInst = table.Column<bool>(type: "boolean", nullable: false),
                    FaseRevestimento = table.Column<bool>(type: "boolean", nullable: false),
                    FasePintura = table.Column<bool>(type: "boolean", nullable: false),
                    FaseServicosComp = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relatos_visita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_relatos_visita_obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "termos_conclusao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObraId = table.Column<int>(type: "integer", nullable: false),
                    DataConclusao = table.Column<DateOnly>(type: "date", nullable: false),
                    Declaracao = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_termos_conclusao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_termos_conclusao_obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assinaturas_relato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelatoVisitaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Papel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas_relato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinaturas_relato_relatos_visita_RelatoVisitaId",
                        column: x => x.RelatoVisitaId,
                        principalTable: "relatos_visita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "imagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelatoVisitaId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_imagens_relatos_visita_RelatoVisitaId",
                        column: x => x.RelatoVisitaId,
                        principalTable: "relatos_visita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assinaturas_termo_conclusao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TermoConclusaoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Papel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas_termo_conclusao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinaturas_termo_conclusao_termos_conclusao_TermoConclusao~",
                        column: x => x.TermoConclusaoId,
                        principalTable: "termos_conclusao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_obra_ObraId_UsuarioId",
                table: "assinaturas_obra",
                columns: new[] { "ObraId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_relato_RelatoVisitaId_UsuarioId",
                table: "assinaturas_relato",
                columns: new[] { "RelatoVisitaId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_termo_conclusao_TermoConclusaoId_UsuarioId",
                table: "assinaturas_termo_conclusao",
                columns: new[] { "TermoConclusaoId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_imagens_RelatoVisitaId",
                table: "imagens",
                column: "RelatoVisitaId");

            migrationBuilder.CreateIndex(
                name: "IX_obras_NumeroCaderneta",
                table: "obras",
                column: "NumeroCaderneta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_relatos_visita_ObraId",
                table: "relatos_visita",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_termos_conclusao_ObraId",
                table: "termos_conclusao",
                column: "ObraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Cpf",
                table: "usuarios",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assinaturas_obra");

            migrationBuilder.DropTable(
                name: "assinaturas_relato");

            migrationBuilder.DropTable(
                name: "assinaturas_termo_conclusao");

            migrationBuilder.DropTable(
                name: "imagens");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "termos_conclusao");

            migrationBuilder.DropTable(
                name: "relatos_visita");

            migrationBuilder.DropTable(
                name: "obras");
        }
    }
}
