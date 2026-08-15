using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.Registros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Authorize]
public class RegistrosController : ControllerBase
{
    private readonly CriarRegistroUseCase _criarRegistro;
    private readonly ListarRegistrosUseCase _listarRegistros;
    private readonly ObterRegistroUseCase _obterRegistro;
    private readonly AdicionarImagemUseCase _adicionarImagem;
    private readonly AssinarRegistroUseCase _assinarRegistro;

    public RegistrosController(
        CriarRegistroUseCase criarRegistro, ListarRegistrosUseCase listarRegistros,
        ObterRegistroUseCase obterRegistro, AdicionarImagemUseCase adicionarImagem,
        AssinarRegistroUseCase assinarRegistro)
    {
        _criarRegistro = criarRegistro;
        _listarRegistros = listarRegistros;
        _obterRegistro = obterRegistro;
        _adicionarImagem = adicionarImagem;
        _assinarRegistro = assinarRegistro;
    }

    [HttpGet("api/obras/{obraId:int}/registros")]
    public async Task<ActionResult<List<RegistroResponse>>> ListarPorObra(int obraId, CancellationToken ct) =>
        Ok(await _listarRegistros.ExecutarAsync(obraId, ct));

    [HttpGet("api/registros/{id:int}")]
    public async Task<ActionResult<RegistroResponse>> Obter(int id, CancellationToken ct) =>
        Ok(await _obterRegistro.ExecutarAsync(id, ct));

    [HttpPost("api/registros")]
    public async Task<ActionResult<RegistroResponse>> Criar(CriarRegistroRequest request, CancellationToken ct) =>
        Ok(await _criarRegistro.ExecutarAsync(request, ct));

    [HttpPost("api/registros/{id:int}/imagens")]
    [RequestSizeLimit(15_000_000)] // 15 MB por foto
    public async Task<ActionResult<ImagemResponse>> AdicionarImagem(int id, IFormFile arquivo, CancellationToken ct)
    {
        await using var stream = arquivo.OpenReadStream();
        var resultado = await _adicionarImagem.ExecutarAsync(
            new AdicionarImagemRequest(id, arquivo.FileName, arquivo.ContentType, stream), ct);
        return Ok(resultado);
    }

    [HttpPost("api/registros/{id:int}/assinar")]
    public async Task<ActionResult<RegistroResponse>> Assinar(int id, CancellationToken ct) =>
        Ok(await _assinarRegistro.ExecutarAsync(id, ct));
}
