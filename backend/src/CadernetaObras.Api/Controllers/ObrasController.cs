using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.Obras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/obras")]
[Authorize]
public class ObrasController : ControllerBase
{
    private readonly CriarObraUseCase _criarObra;
    private readonly ListarObrasUseCase _listarObras;
    private readonly ObterObraUseCase _obterObra;
    private readonly AssinarObraUseCase _assinarObra;

    public ObrasController(
        CriarObraUseCase criarObra, ListarObrasUseCase listarObras,
        ObterObraUseCase obterObra, AssinarObraUseCase assinarObra)
    {
        _criarObra = criarObra;
        _listarObras = listarObras;
        _obterObra = obterObra;
        _assinarObra = assinarObra;
    }

    [HttpGet]
    public async Task<ActionResult<List<ObraResponse>>> Listar(CancellationToken ct) =>
        Ok(await _listarObras.ExecutarAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ObraResponse>> Obter(int id, CancellationToken ct) =>
        Ok(await _obterObra.ExecutarAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ObraResponse>> Criar(CriarObraRequest request, CancellationToken ct) =>
        Ok(await _criarObra.ExecutarAsync(request, ct));

    [HttpPost("{id:int}/assinar")]
    public async Task<ActionResult<ObraResponse>> Assinar(int id, CancellationToken ct) =>
        Ok(await _assinarObra.ExecutarAsync(id, ct));
}
