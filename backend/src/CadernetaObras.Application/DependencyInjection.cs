using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.Auditoria;
using CadernetaObras.Application.Auth;
using CadernetaObras.Application.Obras;
using CadernetaObras.Application.Registros;
using CadernetaObras.Application.TermosConclusao;
using CadernetaObras.Application.Usuarios;
using Microsoft.Extensions.DependencyInjection;

namespace CadernetaObras.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<ListarAuditoriaUseCase>();

        services.AddScoped<CriarUsuarioUseCase>();
        services.AddScoped<ListarUsuariosUseCase>();

        services.AddScoped<CriarObraUseCase>();
        services.AddScoped<ListarObrasUseCase>();
        services.AddScoped<ObterObraUseCase>();

        services.AddScoped<CriarRegistroUseCase>();
        services.AddScoped<ListarRegistrosUseCase>();
        services.AddScoped<ObterRegistroUseCase>();
        services.AddScoped<AdicionarImagemUseCase>();
        services.AddScoped<ObterUrlImagemUseCase>();

        services.AddScoped<CriarTermoUseCase>();
        services.AddScoped<ObterTermoPorObraUseCase>();

        services.AddScoped<AssinarObraUseCase>();
        services.AddScoped<AssinarRegistroUseCase>();
        services.AddScoped<AssinarTermoUseCase>();

        return services;
    }
}
