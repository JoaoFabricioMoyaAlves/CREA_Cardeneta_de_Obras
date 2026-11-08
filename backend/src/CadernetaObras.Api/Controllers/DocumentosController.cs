using CadernetaObras.Application.Common;
using CadernetaObras.Application.Documentos;
using CadernetaObras.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/documentos")]
[Authorize]
public class DocumentosController : ControllerBase
{
    private readonly ObterDocumentoUseCase _obter;
    private readonly AssinarDocumentoUseCase _assinar;

    public DocumentosController(ObterDocumentoUseCase obter, AssinarDocumentoUseCase assinar)
    {
        _obter = obter;
        _assinar = assinar;
    }

    [HttpGet("{tipo}/{referenciaId:int}")]
    public async Task<ActionResult<DocumentoResponse>> Obter(string tipo, int referenciaId, CancellationToken ct) =>
        Ok(await _obter.ExecutarAsync(Converter(tipo), referenciaId, ct));

    [HttpPost("{tipo}/{referenciaId:int}/assinar")]
    public async Task<ActionResult<DocumentoResponse>> Assinar(
        string tipo, int referenciaId, AssinarDocumentoRequest request, CancellationToken ct) =>
        Ok(await _assinar.ExecutarAsync(Converter(tipo), referenciaId, request, ct));

    private static TipoDocumento Converter(string tipo) => tipo switch
    {
        "abertura-obra" => TipoDocumento.AberturaObra,
        "relato-visita" => TipoDocumento.RelatoVisita,
        "termo-conclusao" => TipoDocumento.TermoConclusao,
        _ => throw new NotFoundException("Tipo de documento não encontrado."),
    };
}
