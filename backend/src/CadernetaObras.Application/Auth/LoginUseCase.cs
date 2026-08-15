using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Auth;

public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUseCase(
        IUsuarioRepository usuarios, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService,
        IAuditLogger auditLogger, IUnitOfWork unitOfWork)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> ExecutarAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObterPorCpfAsync(request.Cpf, ct);

        // Mensagem de erro genérica de propósito: não revela se o CPF existe
        // ou se foi a senha que errou (evita enumeração de usuários).
        if (usuario is null || !_passwordHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            _auditLogger.Registrar("LoginFalhou", usuarioId: null, detalhes: $"CPF tentado: {request.Cpf}");
            await _unitOfWork.SalvarAsync(ct);
            throw new UnauthorizedAppException("CPF ou senha inválidos.");
        }

        var token = _jwtTokenService.GerarToken(usuario);

        _auditLogger.Registrar("LoginRealizado", usuario.Id, "Usuario", usuario.Id.ToString());
        await _unitOfWork.SalvarAsync(ct);

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
