using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Domain.Interfaces;

public interface IRelatoVisitaRepository
{
    Task<RelatoVisita?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<RelatoVisita?> ObterComAssinaturasAsync(int id, CancellationToken ct = default);
    Task<List<RelatoVisita>> ListarPorObraAsync(int obraId, CancellationToken ct = default);
    void Adicionar(RelatoVisita relato);
}
