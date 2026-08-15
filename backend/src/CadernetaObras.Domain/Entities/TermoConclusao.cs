using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class TermoConclusao
{
    public int Id { get; set; }
    public int ObraId { get; set; }

    public DateOnly DataConclusao { get; set; }
    public string Declaracao { get; set; } = string.Empty;

    public StatusRegistro Status { get; set; } = StatusRegistro.PendenteAssinatura;

    public Obra? Obra { get; set; }
    public ICollection<AssinaturaTermoConclusao> Assinaturas { get; set; } =
        new List<AssinaturaTermoConclusao>();
}
