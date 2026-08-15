using System.Globalization;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Obras;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Assinaturas;

// Motor de assinatura da Obra: gera hash SHA-256 do conteúdo exato do
// registro no momento da assinatura, vincula ao usuário autenticado, IP e
// user-agent capturados pela API (nunca informados pelo cliente), e
// timestamp do servidor (nunca do navegador). Isso é o que dá à assinatura
// o nível "eletrônica avançada" da Lei 14.063/2020.
public class AssinarObraUseCase
{
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;
    private readonly ITimestampAuthorityService _tsa;

    public AssinarObraUseCase(
        IObraRepository obras, IUsuarioRepository usuarios, IUnitOfWork unitOfWork,
        IHashService hashService, ICurrentUserService currentUser, IAuditLogger auditLogger,
        ITimestampAuthorityService tsa)
    {
        _obras = obras;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _hashService = hashService;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
        _tsa = tsa;
    }

    public async Task<ObraResponse> ExecutarAsync(int obraId, CancellationToken ct = default)
    {
        var obra = await _obras.ObterComAssinaturasAsync(obraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        if (obra.Status != StatusObra.PendenteAssinatura)
            throw new EntidadeImutavelException("Obra");

        var papel = DeterminarPapel(obra, _currentUser.UsuarioId, _currentUser.Perfil);

        if (obra.Assinaturas.Any(a => a.UsuarioId == _currentUser.UsuarioId))
            throw new AssinaturaDuplicadaException();

        var conteudoCanonico = MontarConteudoCanonico(obra);
        var hash = _hashService.GerarHashSha256(conteudoCanonico);
        var carimbo = await _tsa.ObterCarimboAsync(Convert.FromHexString(hash), ct);

        var assinatura = new AssinaturaObra
        {
            ObraId = obra.Id,
            UsuarioId = _currentUser.UsuarioId,
            Papel = papel,
            Data = DateTime.UtcNow,
            CodHash = hash,
            Ip = _currentUser.Ip,
            UserAgent = _currentUser.UserAgent,
            TsaToken = carimbo?.TokenBase64,
            TsaDataHora = carimbo?.DataHoraTsa,
            TsaAutoridade = carimbo?.Autoridade,
        };

        obra.Assinaturas.Add(assinatura);

        _auditLogger.Registrar("ObraAssinada", _currentUser.UsuarioId, "Obra", obra.Id.ToString(), $"Papel: {papel}");

        var assinouEngenheiro = obra.Assinaturas.Any(a => a.Papel == PapelAssinatura.Engenheiro);
        var assinouProprietario = obra.Assinaturas.Any(a => a.Papel == PapelAssinatura.Proprietario);
        if (assinouEngenheiro && assinouProprietario)
        {
            obra.Status = StatusObra.Ativa;
            _auditLogger.Registrar("ObraAtivada", _currentUser.UsuarioId, "Obra", obra.Id.ToString());
        }

        await _unitOfWork.SalvarAsync(ct);

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);
        return ObraMapper.ToResponse(obra, profissional!, proprietario!);
    }

    private static PapelAssinatura DeterminarPapel(Obra obra, Guid usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == usuarioId)
            return PapelAssinatura.Engenheiro;
        if (perfil == PerfilUsuario.Proprietario && obra.IdProprietario == usuarioId)
            return PapelAssinatura.Proprietario;

        throw new ForbiddenException("Você não é Engenheiro ou Proprietário atribuído a esta obra.");
    }

    private static string MontarConteudoCanonico(Obra obra)
    {
        // Concatenação determinística dos campos que compõem "o que está
        // sendo assinado" — qualquer alteração posterior nesses dados
        // quebraria o hash, expondo a adulteração.
        return string.Join('|',
            obra.Id,
            obra.NumeroCaderneta,
            obra.IdProfissional,
            obra.IdProprietario,
            obra.LocalObra,
            obra.NumeroRt,
            obra.AreaTotalEdificadaM2.ToString(CultureInfo.InvariantCulture),
            obra.TipoEdificacao,
            obra.ValorObra.ToString(CultureInfo.InvariantCulture),
            obra.DataReciboAbertura.ToString("O", CultureInfo.InvariantCulture));
    }
}
