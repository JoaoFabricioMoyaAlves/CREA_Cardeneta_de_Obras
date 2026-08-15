using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class AssinaturaTermoConclusao
{
    public int Id { get; set; }
    public int TermoConclusaoId { get; set; }
    public Guid UsuarioId { get; set; }
    public PapelAssinatura Papel { get; set; }

    public DateTime Data { get; set; }
    public string CodHash { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    public TermoConclusao? TermoConclusao { get; set; }
}
