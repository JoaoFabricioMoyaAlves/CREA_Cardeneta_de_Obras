using CadernetaObras.Application.Documentos;
using CadernetaObras.Application.Auditoria;
using CadernetaObras.Application.Auth;
using CadernetaObras.Application.Notificacoes;
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
        services.AddScoped<INotificacaoService, NotificacaoService>();

        services.AddScoped<LoginUseCase>();
        services.AddScoped<SolicitarRecuperacaoSenhaUseCase>();
        services.AddScoped<RedefinirSenhaUseCase>();
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

        services.AddScoped<IDocumentoHandler, AberturaObraHandler>();
        services.AddScoped<IDocumentoHandler, RelatoVisitaHandler>();
        services.AddScoped<IDocumentoHandler, TermoConclusaoHandler>();
        services.AddScoped<ObterDocumentoUseCase>();
        services.AddScoped<AssinarDocumentoUseCase>();
        services.AddSingleton<ILimiteTentativasAssinatura, LimiteTentativasAssinatura>();

        return services;
    }
}
