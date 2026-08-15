using CadernetaObras.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    public AuthController(LoginUseCase loginUseCase) => _loginUseCase = loginUseCase;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var resultado = await _loginUseCase.ExecutarAsync(request, ct);
        return Ok(resultado);
    }
}
