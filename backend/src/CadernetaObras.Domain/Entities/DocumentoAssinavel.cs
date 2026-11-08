using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

// Dados de um documento congelados na primeira assinatura. O PDF nunca é
// guardado: ao visualizar ou imprimir, DadosJson preenche o modelo indicado
// por ModeloVersao. Depois de criada, a linha nunca é alterada.
public class DocumentoAssinavel
{
    public int Id { get; set; }
    public TipoDocumento Tipo { get; set; }
    public int ReferenciaId { get; set; }
    public int ModeloVersao { get; set; }

    // Guardado como texto (nunca jsonb): o hash é calculado sobre estes bytes
    // exatos, e o jsonb reordenaria os campos.
    public string DadosJson { get; set; } = string.Empty;
    public string HashDocumento { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
