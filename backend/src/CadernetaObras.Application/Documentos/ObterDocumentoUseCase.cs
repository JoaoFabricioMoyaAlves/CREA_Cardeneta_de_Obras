using System.Text.Json;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Documentos;

public class ObterDocumentoUseCase
{
    private readonly IEnumerable<IDocumentoHandler> _handlers;
    private readonly IDocumentoAssinavelRepository _documentos;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;
    private readonly IHashService _hashService;

    public ObterDocumentoUseCase(
        IEnumerable<IDocumentoHandler> handlers, IDocumentoAssinavelRepository documentos,
        IUsuarioRepository usuarios, ICurrentUserService currentUser, IHashService hashService)
    {
        _handlers = handlers;
        _documentos = documentos;
        _usuarios = usuarios;
        _currentUser = currentUser;
        _hashService = hashService;
    }

    public async Task<DocumentoResponse> ExecutarAsync(TipoDocumento tipo, int referenciaId, CancellationToken ct = default)
    {
        var handler = _handlers.Single(h => h.Tipo == tipo);
        var contexto = await handler.CarregarAsync(referenciaId, ct);

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && contexto.Obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && contexto.Obra.IdProprietario == _currentUser.UsuarioId);
        if (!podeVer)
            throw new ForbiddenException("Este documento não está atribuído ao seu usuário.");

        var congelado = await _documentos.ObterAsync(tipo, referenciaId, ct);
        var dadosJson = congelado?.DadosJson ?? await handler.MontarDadosJsonAsync(contexto, ct);
        var hash = congelado?.HashDocumento ?? _hashService.GerarHashSha256(dadosJson);

        return await DocumentoResponseBuilder.MontarAsync(
            handler, contexto, tipo, referenciaId, congelado, dadosJson, hash,
            _usuarios, _currentUser.UsuarioId, _currentUser.Perfil, ct);
    }
}

internal static class DocumentoResponseBuilder
{
    public static async Task<DocumentoResponse> MontarAsync(
        IDocumentoHandler handler, ContextoDocumento contexto, TipoDocumento tipo, int referenciaId,
        DocumentoAssinavel? congelado, string dadosJson, string hash,
        IUsuarioRepository usuarios, Guid usuarioId, PerfilUsuario perfil, CancellationToken ct)
    {
        var assinaturas = new List<AssinaturaDocumentoDto>();
        foreach (var a in contexto.Assinaturas.OrderBy(a => a.Data))
        {
            var nome = a.UsuarioId == contexto.Profissional.Id ? contexto.Profissional.Nome
                : a.UsuarioId == contexto.Proprietario.Id ? contexto.Proprietario.Nome
                : (await usuarios.ObterPorIdAsync(a.UsuarioId, ct))?.Nome ?? "Usuário não encontrado";
            assinaturas.Add(new AssinaturaDocumentoDto(a.Papel.ToString(), nome, a.Data, a.HashAssinatura ?? a.CodHash, a.Ip, a.Carimbo, a.Autoridade));
        }

        using var json = JsonDocument.Parse(dadosJson);

        return new DocumentoResponse(
            tipo.ToString(),
            referenciaId,
            congelado?.ModeloVersao ?? DocumentoJson.ModeloVersaoAtual,
            congelado is not null,
            congelado is null && contexto.Assinaturas.Count > 0,
            hash,
            json.RootElement.Clone(),
            assinaturas,
            PapelQuePodeAssinar(handler, contexto, usuarioId, perfil));
    }

    private static string? PapelQuePodeAssinar(IDocumentoHandler handler, ContextoDocumento contexto, Guid usuarioId, PerfilUsuario perfil)
    {
        if (!contexto.Pendente || contexto.Assinaturas.Any(a => a.UsuarioId == usuarioId))
            return null;
        try
        {
            return handler.DeterminarPapel(contexto, usuarioId, perfil).ToString();
        }
        catch (ForbiddenException)
        {
            return null;
        }
    }
}
