using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Infrastructure.Auditoria;

public class AuditLogger : IAuditLogger
{
    private readonly ILogAuditoriaRepository _logs;
    private readonly ICurrentUserService _currentUser;

    public AuditLogger(ILogAuditoriaRepository logs, ICurrentUserService currentUser)
    {
        _logs = logs;
        _currentUser = currentUser;
    }

    public void Registrar(string acao, Guid? usuarioId, string? entidadeTipo = null, string? entidadeId = null, string? detalhes = null)
    {
        _logs.Adicionar(new LogAuditoria
        {
            DataHoraUtc = DateTime.UtcNow,
            UsuarioId = usuarioId,
            Acao = acao,
            EntidadeTipo = entidadeTipo,
            EntidadeId = entidadeId,
            Detalhes = detalhes,
            Ip = _currentUser.Ip,
            UserAgent = _currentUser.UserAgent,
        });
    }
}
