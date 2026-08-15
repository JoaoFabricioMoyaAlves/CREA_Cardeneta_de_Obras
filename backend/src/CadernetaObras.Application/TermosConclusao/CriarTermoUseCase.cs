using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Obras;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.TermosConclusao;

public class CriarTermoUseCase
{
    private readonly ITermoConclusaoRepository _termos;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;

    public CriarTermoUseCase(
        ITermoConclusaoRepository termos, IObraRepository obras, IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IAuditLogger auditLogger)
    {
        _termos = termos;
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task<TermoResponse> ExecutarAsync(CriarTermoRequest request, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Engenheiro)
            throw new ForbiddenException("Somente o Engenheiro/Arquiteto responsável pode emitir o termo de conclusão.");

        var obra = await _obras.ObterPorIdAsync(request.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        if (obra.IdProfissional != _currentUser.UsuarioId)
            throw new ForbiddenException("Você não é o responsável técnico desta obra.");

        if (obra.Status != StatusObra.Ativa)
            throw new ValidationAppException("Só é possível finalizar uma caderneta que esteja ativa.");

        if (await _termos.ObterPorObraIdAsync(obra.Id, ct) is not null)
            throw new ValidationAppException("Esta caderneta já possui um termo de conclusão emitido.");

        var termo = new TermoConclusao
        {
            ObraId = obra.Id,
            DataConclusao = request.DataConclusao,
            Declaracao = request.Declaracao.Trim(),
            Status = StatusRegistro.PendenteAssinatura,
        };

        _termos.Adicionar(termo);
        await _unitOfWork.SalvarAsync(ct); // gera termo.Id

        _auditLogger.Registrar("TermoConclusaoCriado", _currentUser.UsuarioId, "TermoConclusao", termo.Id.ToString(), $"ObraId: {obra.Id}");
        await _unitOfWork.SalvarAsync(ct);

        return new TermoResponse(termo.Id, termo.ObraId, termo.DataConclusao, termo.Declaracao, termo.Status.ToString(), []);
    }
}
