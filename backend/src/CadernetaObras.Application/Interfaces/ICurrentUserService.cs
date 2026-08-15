using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Application.Interfaces;

// Abstrai "quem está fazendo a requisição agora" para os use cases, sem que
// a Application precise conhecer HttpContext (conceito só existe na API).
public interface ICurrentUserService
{
    Guid UsuarioId { get; }
    PerfilUsuario Perfil { get; }
    string Ip { get; }
    string UserAgent { get; }
}
