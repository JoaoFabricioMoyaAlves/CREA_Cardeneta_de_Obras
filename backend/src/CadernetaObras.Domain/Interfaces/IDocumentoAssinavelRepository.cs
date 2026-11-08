using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Interfaces;

public interface IDocumentoAssinavelRepository
{
    Task<DocumentoAssinavel?> ObterAsync(TipoDocumento tipo, int referenciaId, CancellationToken ct = default);
    void Adicionar(DocumentoAssinavel documento);
}
