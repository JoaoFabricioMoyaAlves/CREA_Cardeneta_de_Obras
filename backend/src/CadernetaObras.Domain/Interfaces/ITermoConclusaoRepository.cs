using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Domain.Interfaces;

public interface ITermoConclusaoRepository
{
    Task<TermoConclusao?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<TermoConclusao?> ObterComAssinaturasAsync(int id, CancellationToken ct = default);
    Task<TermoConclusao?> ObterPorObraIdAsync(int obraId, CancellationToken ct = default);
    void Adicionar(TermoConclusao termo);
}
