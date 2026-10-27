using CadernetaObras.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly SolicitarRecuperacaoSenhaUseCase _solicitarRecuperacao;
    private readonly RedefinirSenhaUseCase _redefinirSenha;

    public AuthController(
        LoginUseCase loginUseCase, SolicitarRecuperacaoSenhaUseCase solicitarRecuperacao, RedefinirSenhaUseCase redefinirSenha)
    {
        _loginUseCase = loginUseCase;
        _solicitarRecuperacao = solicitarRecuperacao;
        _redefinirSenha = redefinirSenha;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var resultado = await _loginUseCase.ExecutarAsync(request, ct);
        return Ok(resultado);
    }

    // Sempre 204, exista ou não o e-mail (não revela quem está cadastrado).
    [HttpPost("recuperar-senha")]
    public async Task<IActionResult> RecuperarSenha(SolicitarRecuperacaoSenhaRequest request, CancellationToken ct)
    {
        await _solicitarRecuperacao.ExecutarAsync(request, ct);
        return NoContent();
    }

    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaRequest request, CancellationToken ct)
    {
        await _redefinirSenha.ExecutarAsync(request, ct);
        return NoContent();
    }
}
