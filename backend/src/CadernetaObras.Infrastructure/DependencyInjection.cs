using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Interfaces;
using CadernetaObras.Infrastructure.Auth;
using CadernetaObras.Infrastructure.Persistence;
using CadernetaObras.Infrastructure.Persistence.Repositories;
using CadernetaObras.Infrastructure.Services;
using CadernetaObras.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CadernetaObras.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.SectionName));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IObraRepository, ObraRepository>();
        services.AddScoped<IRelatoVisitaRepository, RelatoVisitaRepository>();
        services.AddScoped<ITermoConclusaoRepository, TermoConclusaoRepository>();

        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IHashService, Sha256HashService>();
        services.AddSingleton<IStorageService, MinioStorageService>();
        services.AddSingleton<IPdfService, QuestPdfService>();

        return services;
    }
}
