using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class DocumentoAssinavelRepository : IDocumentoAssinavelRepository
{
    private readonly AppDbContext _db;
    public DocumentoAssinavelRepository(AppDbContext db) => _db = db;

    public Task<DocumentoAssinavel?> ObterAsync(TipoDocumento tipo, int referenciaId, CancellationToken ct = default) =>
        _db.DocumentosAssinaveis.FirstOrDefaultAsync(d => d.Tipo == tipo && d.ReferenciaId == referenciaId, ct);

    public void Adicionar(DocumentoAssinavel documento) => _db.DocumentosAssinaveis.Add(documento);
}
