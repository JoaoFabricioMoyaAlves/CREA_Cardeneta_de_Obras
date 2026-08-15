namespace CadernetaObras.Application.Auditoria;

public record LogAuditoriaResponse(
    int Id,
    DateTime DataHoraUtc,
    string? UsuarioNome,
    string? UsuarioPerfil,
    string Acao,
    string? EntidadeTipo,
    string? EntidadeId,
    string? Detalhes,
    string Ip
);
