using System.Security.Claims;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Common;
using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext Context =>
        _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Nenhum HttpContext disponível.");

    public Guid UsuarioId
    {
        get
        {
            var claim = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claim is null || !Guid.TryParse(claim, out var id))
                throw new UnauthorizedAppException("Token inválido ou ausente.");
            return id;
        }
    }

    public PerfilUsuario Perfil
    {
        get
        {
            var claim = Context.User.FindFirstValue("perfil");
            if (claim is null || !Enum.TryParse<PerfilUsuario>(claim, out var perfil))
                throw new UnauthorizedAppException("Token inválido ou ausente.");
            return perfil;
        }
    }

    // Considera X-Forwarded-For quando a API está atrás de um reverse proxy
    // (Nginx na VPS) — sem isso, o IP capturado seria sempre o do proxy.
    public string Ip
    {
        get
        {
            var forwarded = Context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
                return forwarded.Split(',')[0].Trim();

            return Context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
        }
    }

    public string UserAgent => Context.Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : "desconhecido";
}
