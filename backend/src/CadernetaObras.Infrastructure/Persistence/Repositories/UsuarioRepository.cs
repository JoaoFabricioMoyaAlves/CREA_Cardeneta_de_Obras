using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CadernetaObras.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObterPorCpfAsync(string cpf, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf, ct);

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public Task<List<Usuario>> ListarAsync(CancellationToken ct = default) =>
        _db.Usuarios.OrderBy(u => u.Nome).ToListAsync(ct);

    public Task<List<Usuario>> ListarPorPerfilAsync(PerfilUsuario perfil, CancellationToken ct = default) =>
        _db.Usuarios.Where(u => u.Perfil == perfil).OrderBy(u => u.Nome).ToListAsync(ct);

    public Task<bool> ExisteAdministradorAsync(CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador, ct);

    public void Adicionar(Usuario usuario) => _db.Usuarios.Add(usuario);
}
