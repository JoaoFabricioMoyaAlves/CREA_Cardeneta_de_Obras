using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class Obra
{
    public int Id { get; set; }

    // Código legível exibido ao usuário (ex: "CAD-2026-0148"), gerado no
    // momento da criação. Não é a PK para não acoplar a chave técnica ao
    // formato de exibição.
    public string NumeroCaderneta { get; set; } = string.Empty;

    public Guid IdAdministrador { get; set; }
    public Guid IdProfissional { get; set; }
    public Guid IdProprietario { get; set; }

    public string LocalObra { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string NumeroRt { get; set; } = string.Empty;

    public decimal AreaConstruirM2 { get; set; }
    public decimal AreaAmpliarM2 { get; set; }
    public decimal AreaReformarM2 { get; set; }
    public decimal AreaRegularizarM2 { get; set; }
    public decimal AreaTotalEdificadaM2 { get; set; }

    public string TipoEdificacao { get; set; } = string.Empty;
    public string? TipoEdificacaoOutros { get; set; }

    public bool AtivTecnicaDirecao { get; set; }
    public bool AtivTecnicaExecucao { get; set; }
    public bool AtivTecnicaFiscalizacao { get; set; }
    public bool AtivTecnicaProjeto { get; set; }

    public decimal ValorObra { get; set; }

    public DateOnly DataReciboAbertura { get; set; }

    public string? NomeEmpresa { get; set; }
    public string? CnpjEmpresa { get; set; }

    public StatusObra Status { get; set; } = StatusObra.PendenteAssinatura;

    public ICollection<AssinaturaObra> Assinaturas { get; set; } = new List<AssinaturaObra>();
    public ICollection<RelatoVisita> RelatosVisita { get; set; } = new List<RelatoVisita>();
    public TermoConclusao? TermoConclusao { get; set; }
}
