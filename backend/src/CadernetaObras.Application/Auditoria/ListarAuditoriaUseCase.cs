using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Auditoria;

public class ListarAuditoriaUseCase
{
    private readonly ILogAuditoriaRepository _logs;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ListarAuditoriaUseCase(ILogAuditoriaRepository logs, IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _logs = logs;
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<List<LogAuditoriaResponse>> ExecutarAsync(int limite = 200, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Administrador)
            throw new ForbiddenException("Somente o Administrador do CREA pode consultar o log de auditoria.");

        var logs = await _logs.ListarRecentesAsync(limite, ct);
        var usuarios = await _usuarios.ListarAsync(ct);
        var usuariosPorId = usuarios.ToDictionary(u => u.Id);

        return logs
            .Select(l =>
            {
                usuariosPorId.TryGetValue(l.UsuarioId ?? Guid.Empty, out var usuario);
                return new LogAuditoriaResponse(
                    l.Id, l.DataHoraUtc, usuario?.Nome, usuario?.Perfil.ToString(),
                    l.Acao, l.EntidadeTipo, l.EntidadeId, l.Detalhes, l.Ip);
            })
            .ToList();
    }
}
