using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Registros;

public class CriarRegistroUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;

    public CriarRegistroUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IAuditLogger auditLogger)
    {
        _registros = registros;
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task<RegistroResponse> ExecutarAsync(CriarRegistroRequest request, CancellationToken ct = default)
    {
        if (_currentUser.Perfil != PerfilUsuario.Engenheiro)
            throw new ForbiddenException("Somente o Engenheiro/Arquiteto responsável pode lançar registros de visita.");

        var obra = await _obras.ObterPorIdAsync(request.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        if (obra.IdProfissional != _currentUser.UsuarioId)
            throw new ForbiddenException("Você não é o responsável técnico desta obra.");

        if (obra.Status != StatusObra.Ativa)
            throw new ValidationAppException("A caderneta precisa estar ativa (dupla assinatura de abertura concluída) para receber registros de visita.");

        var registro = new RelatoVisita
        {
            ObraId = obra.Id,
            DataVisita = request.DataVisita,
            PosicaoObra = request.PosicaoObra.Trim(),
            DecisoesOrientacoes = request.DecisoesOrientacoes.Trim(),
            FaseServicosPreliminares = request.FaseServicosPreliminares,
            FaseFundacao = request.FaseFundacao,
            FaseAlvenarias = request.FaseAlvenarias,
            FaseSuperestrutura = request.FaseSuperestrutura,
            FaseCobertura = request.FaseCobertura,
            FaseEsquadriasInst = request.FaseEsquadriasInst,
            FaseRevestimento = request.FaseRevestimento,
            FasePintura = request.FasePintura,
            FaseServicosComp = request.FaseServicosComp,
            Status = StatusRegistro.PendenteAssinatura,
        };

        _registros.Adicionar(registro);
        await _unitOfWork.SalvarAsync(ct); // gera registro.Id

        _auditLogger.Registrar("RegistroVisitaCriado", _currentUser.UsuarioId, "RelatoVisita", registro.Id.ToString(), $"ObraId: {obra.Id}");
        await _unitOfWork.SalvarAsync(ct);

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);

        return RegistroMapper.ToResponse(registro, profissional!, proprietario!);
    }
}
