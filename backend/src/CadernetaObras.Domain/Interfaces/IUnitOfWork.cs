namespace CadernetaObras.Domain.Interfaces;

// Abstrai o SaveChanges do EF Core para a Application layer nunca depender
// diretamente do DbContext (Infrastructure).
public interface IUnitOfWork
{
    Task<int> SalvarAsync(CancellationToken ct = default);
}
