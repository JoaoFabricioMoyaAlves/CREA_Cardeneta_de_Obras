using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class TermoConclusaoRepository : ITermoConclusaoRepository
{
    private readonly AppDbContext _db;
    public TermoConclusaoRepository(AppDbContext db) => _db = db;

    public Task<TermoConclusao?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _db.TermosConclusao.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<TermoConclusao?> ObterComAssinaturasAsync(int id, CancellationToken ct = default) =>
        _db.TermosConclusao.Include(t => t.Assinaturas).FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<TermoConclusao?> ObterPorObraIdAsync(int obraId, CancellationToken ct = default) =>
        _db.TermosConclusao.FirstOrDefaultAsync(t => t.ObraId == obraId, ct);

    public void Adicionar(TermoConclusao termo) => _db.TermosConclusao.Add(termo);
}
