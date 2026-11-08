using System.Globalization;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Documentos;

// Motor único de assinatura. O signatário confirma CPF e senha, e o servidor
// confere se o documento exibido é o documento atual. IP, user-agent e horário
// vêm sempre do servidor, nunca do navegador. Isso é o que dá à assinatura o
// nível "eletrônica avançada" da Lei 14.063/2020.
public class AssinarDocumentoUseCase
{
    private readonly IEnumerable<IDocumentoHandler> _handlers;
    private readonly IDocumentoAssinavelRepository _documentos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogger _auditLogger;
    private readonly ITimestampAuthorityService _tsa;
    private readonly IHashService _hashService;
    private readonly ILimiteTentativasAssinatura _limite;

    public AssinarDocumentoUseCase(
        IEnumerable<IDocumentoHandler> handlers, IDocumentoAssinavelRepository documentos,
        IUsuarioRepository usuarios, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher,
        ICurrentUserService currentUser, IAuditLogger auditLogger, ITimestampAuthorityService tsa,
        IHashService hashService, ILimiteTentativasAssinatura limite)
    {
        _handlers = handlers;
        _documentos = documentos;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
        _tsa = tsa;
        _hashService = hashService;
        _limite = limite;
    }

    public async Task<DocumentoResponse> ExecutarAsync(
        TipoDocumento tipo, int referenciaId, AssinarDocumentoRequest request, CancellationToken ct = default)
    {
        var handler = _handlers.Single(h => h.Tipo == tipo);
        var contexto = await handler.CarregarAsync(referenciaId, ct);
        var usuarioId = _currentUser.UsuarioId;

        if (!contexto.Pendente)
            throw new EntidadeImutavelException(handler.NomeEntidade);

        await ConfirmarIdentidadeAsync(tipo, referenciaId, usuarioId, request, ct);

        var papel = handler.DeterminarPapel(contexto, usuarioId, _currentUser.Perfil);

        if (contexto.Assinaturas.Any(a => a.UsuarioId == usuarioId))
            throw new AssinaturaDuplicadaException();

        var congelado = await _documentos.ObterAsync(tipo, referenciaId, ct);
        var dadosJson = congelado?.DadosJson ?? await handler.MontarDadosJsonAsync(contexto, ct);
        var hashDocumento = congelado?.HashDocumento ?? _hashService.GerarHashSha256(dadosJson);

        if (!string.Equals(hashDocumento, request.HashDocumentoVisualizado, StringComparison.OrdinalIgnoreCase))
            throw new DocumentoAlteradoException();

        var data = DateTime.UtcNow;
        var ip = _currentUser.Ip;

        if (congelado is null)
        {
            congelado = new DocumentoAssinavel
            {
                Tipo = tipo,
                ReferenciaId = referenciaId,
                ModeloVersao = DocumentoJson.ModeloVersaoAtual,
                DadosJson = dadosJson,
                HashDocumento = hashDocumento,
                CriadoEm = data,
            };
            _documentos.Adicionar(congelado);
        }

        var hashAssinatura = _hashService.GerarHashSha256(
            $"{hashDocumento}|{usuarioId}|{papel}|{data.ToString("O", CultureInfo.InvariantCulture)}|{ip}");
        var carimbo = await _tsa.ObterCarimboAsync(Convert.FromHexString(hashAssinatura), ct);

        handler.AdicionarAssinatura(contexto, new NovaAssinatura(
            usuarioId, papel, data, hashDocumento, hashAssinatura, ip, _currentUser.UserAgent, carimbo));
        handler.ConcluirSeCompleto(contexto, usuarioId);

        await _unitOfWork.SalvarAsync(ct);
        _limite.Limpar(usuarioId);

        await handler.NotificarAsync(contexto, papel, ct);

        return await DocumentoResponseBuilder.MontarAsync(
            handler, contexto, tipo, referenciaId, congelado, dadosJson, hashDocumento,
            _usuarios, usuarioId, _currentUser.Perfil, ct);
    }

    private async Task ConfirmarIdentidadeAsync(
        TipoDocumento tipo, int referenciaId, Guid usuarioId, AssinarDocumentoRequest request, CancellationToken ct)
    {
        if (_limite.Bloqueado(usuarioId))
            throw new ValidationAppException("Muitas tentativas incorretas. A assinatura ficará bloqueada por até 15 minutos.");

        var usuario = await _usuarios.ObterPorIdAsync(usuarioId, ct)
            ?? throw new NotFoundException("Usuário não encontrado.");

        var confirmado = SomenteDigitos(request.Cpf) == SomenteDigitos(usuario.Cpf)
            && !string.IsNullOrEmpty(request.Senha)
            && _passwordHasher.Verificar(request.Senha, usuario.SenhaHash);

        if (confirmado)
            return;

        _limite.RegistrarFalha(usuarioId);
        _auditLogger.Registrar("AssinaturaConfirmacaoFalhou", usuarioId, tipo.ToString(), referenciaId.ToString());
        await _unitOfWork.SalvarAsync(ct);

        // 400, não 401: senha errada aqui não pode derrubar a sessão do usuário.
        throw new ValidationAppException("CPF ou senha incorretos. Confira os dados e tente novamente.");
    }

    private static string SomenteDigitos(string? texto) =>
        new((texto ?? string.Empty).Where(char.IsDigit).ToArray());
}
