using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Domain.Interfaces;

public interface ILogAuditoriaRepository
{
    void Adicionar(LogAuditoria log);
    Task<List<LogAuditoria>> ListarRecentesAsync(int limite, CancellationToken ct = default);
}
