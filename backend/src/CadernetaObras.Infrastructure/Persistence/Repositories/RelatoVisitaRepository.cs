using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class RelatoVisitaRepository : IRelatoVisitaRepository
{
    private readonly AppDbContext _db;
    public RelatoVisitaRepository(AppDbContext db) => _db = db;

    public Task<RelatoVisita?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _db.RelatosVisita.Include(r => r.Imagens).FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<RelatoVisita?> ObterComAssinaturasAsync(int id, CancellationToken ct = default) =>
        _db.RelatosVisita
            .Include(r => r.Imagens)
            .Include(r => r.Assinaturas)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<List<RelatoVisita>> ListarPorObraAsync(int obraId, CancellationToken ct = default) =>
        _db.RelatosVisita
            .Include(r => r.Imagens)
            .Include(r => r.Assinaturas)
            .Where(r => r.ObraId == obraId)
            .OrderByDescending(r => r.DataVisita)
            .ToListAsync(ct);

    public void Adicionar(RelatoVisita relato) => _db.RelatosVisita.Add(relato);
}
