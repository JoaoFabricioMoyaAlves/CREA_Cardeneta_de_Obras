using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.TermosConclusao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Authorize]
public class TermosConclusaoController : ControllerBase
{
    private readonly CriarTermoUseCase _criarTermo;
    private readonly ObterTermoPorObraUseCase _obterTermoPorObra;
    private readonly AssinarTermoUseCase _assinarTermo;

    public TermosConclusaoController(
        CriarTermoUseCase criarTermo, ObterTermoPorObraUseCase obterTermoPorObra, AssinarTermoUseCase assinarTermo)
    {
        _criarTermo = criarTermo;
        _obterTermoPorObra = obterTermoPorObra;
        _assinarTermo = assinarTermo;
    }

    [HttpGet("api/obras/{obraId:int}/termo-conclusao")]
    public async Task<ActionResult<TermoResponse?>> ObterPorObra(int obraId, CancellationToken ct) =>
        Ok(await _obterTermoPorObra.ExecutarAsync(obraId, ct));

    [HttpPost("api/termos-conclusao")]
    public async Task<ActionResult<TermoResponse>> Criar(CriarTermoRequest request, CancellationToken ct) =>
        Ok(await _criarTermo.ExecutarAsync(request, ct));

    [HttpPost("api/termos-conclusao/{id:int}/assinar")]
    public async Task<ActionResult<TermoResponse>> Assinar(int id, CancellationToken ct) =>
        Ok(await _assinarTermo.ExecutarAsync(id, ct));
}
