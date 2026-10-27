using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Domain.Interfaces;

public interface ITokenSenhaRepository
{
    Task<TokenSenha?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default);
    Task<List<TokenSenha>> ListarPendentesDoUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    void Adicionar(TokenSenha token);
}
