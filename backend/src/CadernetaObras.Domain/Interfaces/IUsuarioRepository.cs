using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> ObterPorCpfAsync(string cpf, CancellationToken ct = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<List<Usuario>> ListarAsync(CancellationToken ct = default);
    Task<List<Usuario>> ListarPorPerfilAsync(PerfilUsuario perfil, CancellationToken ct = default);
    Task<bool> ExisteAdministradorAsync(CancellationToken ct = default);
    void Adicionar(Usuario usuario);
}
