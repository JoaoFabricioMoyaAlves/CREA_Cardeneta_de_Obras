using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

// Registro de assinatura da abertura de uma Obra. Uma linha por signatário
// (Engenheiro e Proprietário) — nunca é editada ou excluída após criada.
public class AssinaturaObra
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public Guid UsuarioId { get; set; }
    public PapelAssinatura Papel { get; set; }

    public DateTime Data { get; set; }
    public string CodHash { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    // Carimbo de tempo RFC 3161 — opcional, nulo quando a TSA externa não
    // respondeu no momento da assinatura (não bloqueia o fluxo).
    public string? TsaToken { get; set; }
    public DateTime? TsaDataHora { get; set; }
    public string? TsaAutoridade { get; set; }

    public Obra? Obra { get; set; }
}
