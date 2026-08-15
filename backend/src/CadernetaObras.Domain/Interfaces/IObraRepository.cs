using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Domain.Interfaces;

public interface IObraRepository
{
    Task<Obra?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Obra?> ObterComAssinaturasAsync(int id, CancellationToken ct = default);
    Task<List<Obra>> ListarVisiveisAsync(Guid usuarioId, CadernetaObras.Domain.Enums.PerfilUsuario perfil, CancellationToken ct = default);
    void Adicionar(Obra obra);
}
