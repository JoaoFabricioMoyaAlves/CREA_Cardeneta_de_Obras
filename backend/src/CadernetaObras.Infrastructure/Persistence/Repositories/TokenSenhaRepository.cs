using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class TokenSenhaRepository : ITokenSenhaRepository
{
    private readonly AppDbContext _db;
    public TokenSenhaRepository(AppDbContext db) => _db = db;

    public Task<TokenSenha?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default) =>
        _db.TokensSenha.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public Task<List<TokenSenha>> ListarPendentesDoUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        _db.TokensSenha.Where(t => t.UsuarioId == usuarioId && t.UsadoEm == null).ToListAsync(ct);

    public void Adicionar(TokenSenha token) => _db.TokensSenha.Add(token);
}
