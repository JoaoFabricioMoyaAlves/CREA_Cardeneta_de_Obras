using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class ObraRepository : IObraRepository
{
    private readonly AppDbContext _db;
    public ObraRepository(AppDbContext db) => _db = db;

    public Task<Obra?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Obras.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Obra?> ObterComAssinaturasAsync(int id, CancellationToken ct = default) =>
        _db.Obras.Include(o => o.Assinaturas).FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<List<Obra>> ListarVisiveisAsync(Guid usuarioId, PerfilUsuario perfil, CancellationToken ct = default)
    {
        var query = _db.Obras.Include(o => o.Assinaturas).AsQueryable();

        query = perfil switch
        {
            PerfilUsuario.Administrador => query,
            PerfilUsuario.Engenheiro => query.Where(o => o.IdProfissional == usuarioId),
            PerfilUsuario.Proprietario => query.Where(o => o.IdProprietario == usuarioId),
            _ => query.Where(_ => false),
        };

        return query.OrderByDescending(o => o.Id).ToListAsync(ct);
    }

    public void Adicionar(Obra obra) => _db.Obras.Add(obra);
}
