using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CadernetaObras.Infrastructure.Persistence;

// Cria o primeiro Administrador a partir de variáveis de ambiente/appsettings
// no start-up, só se ainda não existir nenhum administrador no banco.
// Resolve o problema de ovo-e-galinha: cadastrar usuário exige ser Admin,
// mas o primeiro Admin precisa vir de algum lugar fora do fluxo normal da API.
public static class DbInitializer
{
    public static async Task SeedAdminAsync(AppDbContext db, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        await db.Database.MigrateAsync();

        var jaTemAdmin = await db.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador);
        if (jaTemAdmin) return;

        var cpf = configuration["Bootstrap:AdminCpf"];
        var senha = configuration["Bootstrap:AdminSenha"];
        var nome = configuration["Bootstrap:AdminNome"] ?? "Administrador CREA";
        var email = configuration["Bootstrap:AdminEmail"] ?? "admin@crea.local";
        var telefone = configuration["Bootstrap:AdminTelefone"] ?? "(00) 00000-0000";

        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
            return; // sem credenciais de bootstrap configuradas, não cria nada

        db.Usuarios.Add(new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Cpf = cpf,
            Email = email.ToLowerInvariant(),
            Telefone = telefone,
            Perfil = PerfilUsuario.Administrador,
            SenhaHash = passwordHasher.Hash(senha),
            CriadoEm = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
    }
}
