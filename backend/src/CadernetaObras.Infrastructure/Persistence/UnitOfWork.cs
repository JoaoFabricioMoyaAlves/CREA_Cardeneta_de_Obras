using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CadernetaObras.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private const string ViolacaoDeUnicidade = "23505";

    private readonly AppDbContext _db;
    public UnitOfWork(AppDbContext db) => _db = db;

    public async Task<int> SalvarAsync(CancellationToken ct = default)
    {
        try
        {
            return await _db.SaveChangesAsync(ct);
        }
        // Dois signatários ao mesmo tempo, ou clique duplo no botão de assinar.
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
        {
            SqlState: ViolacaoDeUnicidade,
            TableName: "documentos_assinaveis" or "assinaturas_obra" or "assinaturas_relato" or "assinaturas_termo_conclusao",
        })
        {
            throw new ConflitoConcorrenciaException();
        }
    }
}
