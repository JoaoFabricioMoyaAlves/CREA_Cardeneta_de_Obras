namespace CadernetaObras.Domain.Entities;

public class Imagem
{
    public int Id { get; set; }
    public int RelatoVisitaId { get; set; }
    public DateTime Data { get; set; }

    // Nome original do arquivo enviado pelo usuário (metadado de exibição).
    public string Name { get; set; } = string.Empty;

    // Chave do objeto no bucket MinIO/S3 (não é a URL pública direta).
    public string StorageKey { get; set; } = string.Empty;

    public RelatoVisita? RelatoVisita { get; set; }
}
