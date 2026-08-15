using CadernetaObras.Application.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly CriarUsuarioUseCase _criarUsuario;
    private readonly ListarUsuariosUseCase _listarUsuarios;

    public UsuariosController(CriarUsuarioUseCase criarUsuario, ListarUsuariosUseCase listarUsuarios)
    {
        _criarUsuario = criarUsuario;
        _listarUsuarios = listarUsuarios;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar(CancellationToken ct) =>
        Ok(await _listarUsuarios.ExecutarAsync(ct));

    [HttpPost]
    public async Task<ActionResult<UsuarioCriadoResponse>> Criar(CriarUsuarioRequest request, CancellationToken ct) =>
        Ok(await _criarUsuario.ExecutarAsync(request, ct));
}
