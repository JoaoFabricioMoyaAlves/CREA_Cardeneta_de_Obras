using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Application.Interfaces;

public interface IJwtTokenService
{
    string GerarToken(Usuario usuario);
}
