using System.Globalization;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.TermosConclusao;
using CadernetaObras.Application.Obras;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Assinaturas;

public class AssinarTermoUseCase
{
    private readonly ITermoConclusaoRepository _termos;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;
    private readonly ITimestampAuthorityService _tsa;

    public AssinarTermoUseCase(
        ITermoConclusaoRepository termos, IObraRepository obras, IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork, IHashService hashService, ICurrentUserService currentUser,
        IAuditLogger auditLogger, ITimestampAuthorityService tsa)
    {
        _termos = termos;
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _hashService = hashService;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
        _tsa = tsa;
    }

    public async Task<TermoResponse> ExecutarAsync(int termoId, CancellationToken ct = default)
    {
        var termo = await _termos.ObterComAssinaturasAsync(termoId, ct)
            ?? throw new NotFoundException("Termo de conclusão não encontrado.");

        if (termo.Status != StatusRegistro.PendenteAssinatura)
            throw new EntidadeImutavelException("Termo de conclusão");

        var obra = await _obras.ObterPorIdAsync(termo.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var papel = DeterminarPapel(obra, _currentUser.UsuarioId, _currentUser.Perfil);

        if (termo.Assinaturas.Any(a => a.UsuarioId == _currentUser.UsuarioId))
            throw new AssinaturaDuplicadaException();

        var hash = _hashService.GerarHashSha256(MontarConteudoCanonico(termo));
        var carimbo = await _tsa.ObterCarimboAsync(Convert.FromHexString(hash), ct);

        termo.Assinaturas.Add(new AssinaturaTermoConclusao
        {
            TermoConclusaoId = termo.Id,
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

        _auditLogger.Registrar("TermoConclusaoAssinado", _currentUser.UsuarioId, "TermoConclusao", termo.Id.ToString(), $"Papel: {papel}");

        var assinouEngenheiro = termo.Assinaturas.Any(a => a.Papel == PapelAssinatura.Engenheiro);
        var assinouProprietario = termo.Assinaturas.Any(a => a.Papel == PapelAssinatura.Proprietario);
        if (assinouEngenheiro && assinouProprietario)
        {
            termo.Status = StatusRegistro.Assinado;
            obra.Status = StatusObra.Finalizada; // encerra a caderneta por completo
            _auditLogger.Registrar("ObraFinalizada", _currentUser.UsuarioId, "Obra", obra.Id.ToString());
        }

        await _unitOfWork.SalvarAsync(ct);

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);

        var assinaturas = termo.Assinaturas
            .Select(a => new AssinaturaResumoDto(
                a.Papel.ToString(),
                a.UsuarioId,
                a.UsuarioId == profissional?.Id ? profissional!.Nome : proprietario?.Nome ?? "",
                a.Data,
                a.CodHash,
                a.TsaToken is not null,
                a.TsaAutoridade,
                a.TsaDataHora))
            .ToList();

        return new TermoResponse(termo.Id, termo.ObraId, termo.DataConclusao, termo.Declaracao, termo.Status.ToString(), assinaturas);
    }

    private static PapelAssinatura DeterminarPapel(Obra obra, Guid usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == usuarioId)
            return PapelAssinatura.Engenheiro;
        if (perfil == PerfilUsuario.Proprietario && obra.IdProprietario == usuarioId)
            return PapelAssinatura.Proprietario;

        throw new ForbiddenException("Você não é Engenheiro ou Proprietário atribuído a esta obra.");
    }

    private static string MontarConteudoCanonico(TermoConclusao termo)
    {
        return string.Join('|',
            termo.Id, termo.ObraId,
            termo.DataConclusao.ToString("O", CultureInfo.InvariantCulture),
            termo.Declaracao);
    }
}
