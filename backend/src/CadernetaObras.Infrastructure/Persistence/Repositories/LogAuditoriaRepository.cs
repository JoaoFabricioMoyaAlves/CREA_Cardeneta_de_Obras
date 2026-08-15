using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class LogAuditoriaRepository : ILogAuditoriaRepository
{
    private readonly AppDbContext _db;
    public LogAuditoriaRepository(AppDbContext db) => _db = db;

    public void Adicionar(LogAuditoria log) => _db.LogsAuditoria.Add(log);

    public Task<List<LogAuditoria>> ListarRecentesAsync(int limite, CancellationToken ct = default) =>
        _db.LogsAuditoria.OrderByDescending(l => l.DataHoraUtc).Take(limite).ToListAsync(ct);
}
