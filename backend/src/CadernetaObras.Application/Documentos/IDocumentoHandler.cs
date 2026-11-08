using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Application.Documentos;

public record AssinaturaInfo(
    PapelAssinatura Papel,
    Guid UsuarioId,
    DateTime Data,
    string CodHash,
    string? HashAssinatura,
    string Ip,
    bool Carimbo,
    string? Autoridade,
    DateTime? DataCarimbo);

public record NovaAssinatura(
    Guid UsuarioId,
    PapelAssinatura Papel,
    DateTime Data,
    string CodHash,
    string HashAssinatura,
    string Ip,
    string UserAgent,
    CarimboTempo? Carimbo);

public class ContextoDocumento
{
    public required Obra Obra { get; init; }
    public required Usuario Profissional { get; init; }
    public required Usuario Proprietario { get; init; }
    public required bool Pendente { get; set; }
    public required List<AssinaturaInfo> Assinaturas { get; init; }
    public RelatoVisita? Relato { get; init; }
    public TermoConclusao? Termo { get; init; }

    public bool Assinou(PapelAssinatura papel) => Assinaturas.Any(a => a.Papel == papel);
}

// O que muda de um documento para outro. O fluxo de assinatura em si fica em
// AssinarDocumentoUseCase, num lugar só.
public interface IDocumentoHandler
{
    TipoDocumento Tipo { get; }
    string NomeEntidade { get; }
    Task<ContextoDocumento> CarregarAsync(int referenciaId, CancellationToken ct);
    Task<string> MontarDadosJsonAsync(ContextoDocumento contexto, CancellationToken ct);
    PapelAssinatura DeterminarPapel(ContextoDocumento contexto, Guid usuarioId, PerfilUsuario perfil);
    void AdicionarAssinatura(ContextoDocumento contexto, NovaAssinatura nova);
    void ConcluirSeCompleto(ContextoDocumento contexto, Guid usuarioId);
    Task NotificarAsync(ContextoDocumento contexto, PapelAssinatura papel, CancellationToken ct);
}

internal static class DocumentoHandlerHelpers
{
    public static ProfissionalDocumento Profissional(Usuario u) => new(u.Nome, u.TituloProfissional, u.NumeroRegistro);
    public static EmpresaDocumento Empresa(Obra o) => new(o.NomeEmpresa, o.CnpjEmpresa);
    public static ProprietarioDocumento Proprietario(Usuario u) => new(u.Nome, u.Cpf, u.Telefone);
}
