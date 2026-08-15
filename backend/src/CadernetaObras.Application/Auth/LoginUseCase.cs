using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Auth;

public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(IUsuarioRepository usuarios, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> ExecutarAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObterPorCpfAsync(request.Cpf, ct);

        // Mensagem de erro genérica de propósito: não revela se o CPF existe
        // ou se foi a senha que errou (evita enumeração de usuários).
        if (usuario is null || !_passwordHasher.Verificar(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedAppException("CPF ou senha inválidos.");

        var token = _jwtTokenService.GerarToken(usuario);

        return new LoginResponse(
            token,
            new UsuarioLogadoDto(
                usuario.Id,
                usuario.Nome,
                usuario.Cpf,
                usuario.Email,
                usuario.Perfil.ToString(),
                usuario.TituloProfissional,
                usuario.NumeroRegistro
            )
        );
    }
}
