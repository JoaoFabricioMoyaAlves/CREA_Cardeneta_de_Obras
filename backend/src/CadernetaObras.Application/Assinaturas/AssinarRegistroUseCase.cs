using System.Globalization;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Registros;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Assinaturas;

public class AssinarRegistroUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;
    private readonly ITimestampAuthorityService _tsa;

    public AssinarRegistroUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork, IHashService hashService, ICurrentUserService currentUser,
        IAuditLogger auditLogger, ITimestampAuthorityService tsa)
    {
        _registros = registros;
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _hashService = hashService;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
        _tsa = tsa;
    }

    public async Task<RegistroResponse> ExecutarAsync(int registroId, CancellationToken ct = default)
    {
        var registro = await _registros.ObterComAssinaturasAsync(registroId, ct)
            ?? throw new NotFoundException("Registro de visita não encontrado.");

        if (registro.Status != StatusRegistro.PendenteAssinatura)
            throw new EntidadeImutavelException("Registro de visita");

        var obra = await _obras.ObterPorIdAsync(registro.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var papel = DeterminarPapel(obra, _currentUser.UsuarioId, _currentUser.Perfil);

        if (registro.Assinaturas.Any(a => a.UsuarioId == _currentUser.UsuarioId))
            throw new AssinaturaDuplicadaException();

        var hash = _hashService.GerarHashSha256(MontarConteudoCanonico(registro));
        var carimbo = await _tsa.ObterCarimboAsync(Convert.FromHexString(hash), ct);

        registro.Assinaturas.Add(new AssinaturaRelato
        {
            RelatoVisitaId = registro.Id,
            UsuarioId = _currentUser.UsuarioId,
            Papel = papel,
            Data = DateTime.UtcNow,
            CodHash = hash,
            Ip = _currentUser.Ip,
            UserAgent = _currentUser.UserAgent,
            TsaToken = carimbo?.TokenBase64,
            TsaDataHora = carimbo?.DataHoraTsa,
            TsaAutoridade = carimbo?.Autoridade,
        });

        _auditLogger.Registrar("RegistroVisitaAssinado", _currentUser.UsuarioId, "RelatoVisita", registro.Id.ToString(), $"Papel: {papel}");

        var assinouEngenheiro = registro.Assinaturas.Any(a => a.Papel == PapelAssinatura.Engenheiro);
        var assinouProprietario = registro.Assinaturas.Any(a => a.Papel == PapelAssinatura.Proprietario);
        if (assinouEngenheiro && assinouProprietario)
            registro.Status = StatusRegistro.Assinado; // a partir daqui, imutável (sem endpoint de update/delete)

        await _unitOfWork.SalvarAsync(ct);

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);
        return RegistroMapper.ToResponse(registro, profissional!, proprietario!);
    }

    private static PapelAssinatura DeterminarPapel(Obra obra, Guid usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == usuarioId)
            return PapelAssinatura.Engenheiro;
        if (perfil == PerfilUsuario.Proprietario && obra.IdProprietario == usuarioId)
            return PapelAssinatura.Proprietario;

        throw new ForbiddenException("Você não é Engenheiro ou Proprietário atribuído a esta obra.");
    }

    private static string MontarConteudoCanonico(RelatoVisita registro)
    {
        return string.Join('|',
            registro.Id,
            registro.ObraId,
            registro.DataVisita.ToString("O", CultureInfo.InvariantCulture),
            registro.PosicaoObra,
            registro.DecisoesOrientacoes,
            registro.FaseServicosPreliminares, registro.FaseFundacao, registro.FaseAlvenarias,
            registro.FaseSuperestrutura, registro.FaseCobertura, registro.FaseEsquadriasInst,
            registro.FaseRevestimento, registro.FasePintura, registro.FaseServicosComp);
    }
}
