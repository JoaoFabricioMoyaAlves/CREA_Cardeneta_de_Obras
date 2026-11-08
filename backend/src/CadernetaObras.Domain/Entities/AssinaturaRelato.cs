using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class AssinaturaRelato
{
    public int Id { get; set; }
    public int RelatoVisitaId { get; set; }
    public Guid UsuarioId { get; set; }
    public PapelAssinatura Papel { get; set; }

    public DateTime Data { get; set; }
    public string CodHash { get; set; } = string.Empty;

    // Identifica esta assinatura (documento + signatário + momento + IP). Nulo nas
    // assinaturas anteriores ao novo fluxo.
    public string? HashAssinatura { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    public string? TsaToken { get; set; }
    public DateTime? TsaDataHora { get; set; }
    public string? TsaAutoridade { get; set; }

    public RelatoVisita? RelatoVisita { get; set; }
}
