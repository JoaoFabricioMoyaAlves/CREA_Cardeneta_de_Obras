using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.TermosConclusao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/termos-conclusao")]
[Authorize]
public class TermosConclusaoController : ControllerBase
{
    private readonly CriarTermoUseCase _criarTermo;
    private readonly AssinarTermoUseCase _assinarTermo;

    public TermosConclusaoController(CriarTermoUseCase criarTermo, AssinarTermoUseCase assinarTermo)
    {
        _criarTermo = criarTermo;
        _assinarTermo = assinarTermo;
    }

    [HttpPost]
    public async Task<ActionResult<TermoResponse>> Criar(CriarTermoRequest request, CancellationToken ct) =>
        Ok(await _criarTermo.ExecutarAsync(request, ct));

    [HttpPost("{id:int}/assinar")]
    public async Task<ActionResult<TermoResponse>> Assinar(int id, CancellationToken ct) =>
        Ok(await _assinarTermo.ExecutarAsync(id, ct));
}
