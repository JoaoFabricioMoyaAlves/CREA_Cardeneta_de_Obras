using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class RelatoVisita
{
    public int Id { get; set; }
    public int ObraId { get; set; }

    public DateOnly DataVisita { get; set; }
    public string PosicaoObra { get; set; } = string.Empty;
    public string DecisoesOrientacoes { get; set; } = string.Empty;

    public bool FaseServicosPreliminares { get; set; }
    public bool FaseFundacao { get; set; }
    public bool FaseAlvenarias { get; set; }
    public bool FaseSuperestrutura { get; set; }
    public bool FaseCobertura { get; set; }
    public bool FaseEsquadriasInst { get; set; }
    public bool FaseRevestimento { get; set; }
    public bool FasePintura { get; set; }
    public bool FaseServicosComp { get; set; }

    public StatusRegistro Status { get; set; } = StatusRegistro.PendenteAssinatura;

    public Obra? Obra { get; set; }
    public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();
    public ICollection<AssinaturaRelato> Assinaturas { get; set; } = new List<AssinaturaRelato>();
}
