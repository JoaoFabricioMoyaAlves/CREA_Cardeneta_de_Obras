using CadernetaObras.Application.Auditoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/auditoria")]
[Authorize]
public class AuditoriaController : ControllerBase
{
    private readonly ListarAuditoriaUseCase _listarAuditoria;
    public AuditoriaController(ListarAuditoriaUseCase listarAuditoria) => _listarAuditoria = listarAuditoria;

    [HttpGet]
    public async Task<ActionResult<List<LogAuditoriaResponse>>> Listar([FromQuery] int limite, CancellationToken ct) =>
        Ok(await _listarAuditoria.ExecutarAsync(limite == 0 ? 200 : limite, ct));
}
