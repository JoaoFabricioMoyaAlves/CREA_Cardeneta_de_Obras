using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Auth;

public class RedefinirSenhaUseCase
{
    public const int TamanhoMinimoSenha = 8;

    private readonly IUsuarioRepository _usuarios;
    private readonly ITokenSenhaRepository _tokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogger _auditLogger;

    public RedefinirSenhaUseCase(
        IUsuarioRepository usuarios, ITokenSenhaRepository tokens, IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher, IAuditLogger auditLogger)
    {
        _usuarios = usuarios;
        _tokens = tokens;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _auditLogger = auditLogger;
    }

    public async Task ExecutarAsync(RedefinirSenhaRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < TamanhoMinimoSenha)
            throw new ValidationAppException($"A nova senha precisa ter pelo menos {TamanhoMinimoSenha} caracteres.");

        var token = string.IsNullOrWhiteSpace(request.Token)
            ? null
            : await _tokens.ObterPorHashAsync(TokenSenhaGerador.Hash(request.Token.Trim()), ct);

        if (token is null || token.UsadoEm is not null || token.ExpiraEm < DateTime.UtcNow)
            throw new ValidationAppException("Este link é inválido, já foi usado ou expirou. Solicite um novo em \"Esqueci minha senha\".");

        var usuario = await _usuarios.ObterPorIdAsync(token.UsuarioId, ct)
            ?? throw new ValidationAppException("Este link é inválido, já foi usado ou expirou. Solicite um novo em \"Esqueci minha senha\".");

        usuario.SenhaHash = _passwordHasher.Hash(request.NovaSenha);

        // Marca este e qualquer outro link pendente do usuário como usado.
        foreach (var pendente in await _tokens.ListarPendentesDoUsuarioAsync(usuario.Id, ct))
            pendente.UsadoEm = DateTime.UtcNow;
        token.UsadoEm = DateTime.UtcNow;

        _auditLogger.Registrar("SenhaRedefinida", usuario.Id, "Usuario", usuario.Id.ToString(), $"Via: {token.Finalidade}");
        await _unitOfWork.SalvarAsync(ct);
    }
}
