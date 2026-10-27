using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Auth;

public class SolicitarRecuperacaoSenhaUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ITokenSenhaRepository _tokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacaoService _notificacoes;
    private readonly IAuditLogger _auditLogger;

    public SolicitarRecuperacaoSenhaUseCase(
        IUsuarioRepository usuarios, ITokenSenhaRepository tokens, IUnitOfWork unitOfWork,
        INotificacaoService notificacoes, IAuditLogger auditLogger)
    {
        _usuarios = usuarios;
        _tokens = tokens;
        _unitOfWork = unitOfWork;
        _notificacoes = notificacoes;
        _auditLogger = auditLogger;
    }

    // Sempre termina sem erro, exista ou não o e-mail — a resposta da API é a
    // mesma nos dois casos para não revelar quais e-mails estão cadastrados.
    public async Task ExecutarAsync(SolicitarRecuperacaoSenhaRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email)) return;

        var usuario = await _usuarios.ObterPorEmailAsync(request.Email.Trim(), ct);
        if (usuario is null)
        {
            _auditLogger.Registrar("RecuperacaoSenhaEmailDesconhecido", usuarioId: null, detalhes: $"E-mail tentado: {request.Email.Trim()}");
            await _unitOfWork.SalvarAsync(ct);
            return;
        }

        // Um pedido novo invalida links anteriores ainda não usados.
        foreach (var pendente in await _tokens.ListarPendentesDoUsuarioAsync(usuario.Id, ct))
            pendente.UsadoEm = DateTime.UtcNow;

        var (entidade, token) = TokenSenhaGerador.Criar(usuario.Id, FinalidadeTokenSenha.RecuperacaoSenha);
        _tokens.Adicionar(entidade);
        _auditLogger.Registrar("RecuperacaoSenhaSolicitada", usuario.Id, "Usuario", usuario.Id.ToString());
        await _unitOfWork.SalvarAsync(ct);

        _notificacoes.RecuperacaoSenha(usuario, token);
    }
}
