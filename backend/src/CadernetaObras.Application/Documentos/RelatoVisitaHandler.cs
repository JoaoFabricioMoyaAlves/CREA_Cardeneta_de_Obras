using System.Security.Cryptography;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Application.Registros;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Documentos;

public class RelatoVisitaHandler : IDocumentoHandler
{
    private readonly IRelatoVisitaRepository _relatos;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IStorageService _storage;
    private readonly IAuditLogger _auditLogger;
    private readonly INotificacaoService _notificacoes;

    public RelatoVisitaHandler(
        IRelatoVisitaRepository relatos, IObraRepository obras, IUsuarioRepository usuarios,
        IStorageService storage, IAuditLogger auditLogger, INotificacaoService notificacoes)
    {
        _relatos = relatos;
        _obras = obras;
        _usuarios = usuarios;
        _storage = storage;
        _auditLogger = auditLogger;
        _notificacoes = notificacoes;
    }

    public TipoDocumento Tipo => TipoDocumento.RelatoVisita;
    public string NomeEntidade => "Registro de visita";

    public async Task<ContextoDocumento> CarregarAsync(int referenciaId, CancellationToken ct)
    {
        var relato = await _relatos.ObterComAssinaturasAsync(referenciaId, ct)
            ?? throw new NotFoundException("Registro de visita não encontrado.");
        var obra = await _obras.ObterPorIdAsync(relato.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        return new ContextoDocumento
        {
            Obra = obra,
            Relato = relato,
            Profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct)
                ?? throw new NotFoundException("Responsável técnico da obra não encontrado."),
            Proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct)
                ?? throw new NotFoundException("Proprietário da obra não encontrado."),
            Pendente = relato.Status == StatusRegistro.PendenteAssinatura,
            Assinaturas = relato.Assinaturas
                .Select(a => new AssinaturaInfo(a.Papel, a.UsuarioId, a.Data, a.CodHash, a.HashAssinatura, a.Ip, a.TsaToken is not null, a.TsaAutoridade, a.TsaDataHora))
                .ToList(),
        };
    }

    public async Task<string> MontarDadosJsonAsync(ContextoDocumento c, CancellationToken ct)
    {
        var relato = c.Relato!;
        var idsDaObra = (await _relatos.ListarPorObraAsync(relato.ObraId, ct)).Select(r => r.Id).OrderBy(id => id).ToList();

        var fotos = new List<FotoDocumento>();
        foreach (var imagem in relato.Imagens.OrderBy(i => i.Id))
            fotos.Add(new FotoDocumento(imagem.Id, imagem.Name, await CalcularHashAsync(imagem.StorageKey, c.Pendente, ct)));

        return DocumentoJson.Serializar(new DadosRelato(
            c.Obra.NumeroCaderneta,
            idsDaObra.IndexOf(relato.Id) + 1,
            c.Obra.LocalObra, c.Obra.Cidade, c.Obra.Estado,
            DocumentoHandlerHelpers.Profissional(c.Profissional),
            DocumentoHandlerHelpers.Proprietario(c.Proprietario),
            relato.DataVisita,
            RegistroMapper.ListarFases(relato),
            relato.PosicaoObra,
            relato.DecisoesOrientacoes,
            fotos));
    }

    // Em documento pendente, a falha precisa aparecer: congelar sem o hash da
    // foto deixaria a imagem fora do que foi assinado. Em documento já assinado
    // e sem dados congelados, a falha não pode impedir a leitura.
    private async Task<string?> CalcularHashAsync(string storageKey, bool pendente, CancellationToken ct)
    {
        try
        {
            await using var arquivo = await _storage.ObterArquivoAsync(storageKey, ct);
            return Convert.ToHexStringLower(await SHA256.HashDataAsync(arquivo, ct));
        }
        catch (Exception) when (!pendente)
        {
            return null;
        }
    }

    public PapelAssinatura DeterminarPapel(ContextoDocumento c, Guid usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Engenheiro && c.Obra.IdProfissional == usuarioId)
            return PapelAssinatura.Engenheiro;
        if (perfil == PerfilUsuario.Proprietario && c.Obra.IdProprietario == usuarioId)
            return PapelAssinatura.Proprietario;

        throw new ForbiddenException("Você não é o responsável técnico nem o proprietário desta obra.");
    }

    public void AdicionarAssinatura(ContextoDocumento c, NovaAssinatura n)
    {
        c.Relato!.Assinaturas.Add(new AssinaturaRelato
        {
            RelatoVisitaId = c.Relato.Id, UsuarioId = n.UsuarioId, Papel = n.Papel, Data = n.Data,
            CodHash = n.CodHash, HashAssinatura = n.HashAssinatura, Ip = n.Ip, UserAgent = n.UserAgent,
            TsaToken = n.Carimbo?.TokenBase64, TsaDataHora = n.Carimbo?.DataHoraTsa, TsaAutoridade = n.Carimbo?.Autoridade,
        });
        c.Assinaturas.Add(new AssinaturaInfo(n.Papel, n.UsuarioId, n.Data, n.CodHash, n.HashAssinatura, n.Ip, n.Carimbo is not null, n.Carimbo?.Autoridade, n.Carimbo?.DataHoraTsa));
        _auditLogger.Registrar("RegistroVisitaAssinado", n.UsuarioId, "RelatoVisita", c.Relato.Id.ToString(), $"Papel: {n.Papel}");
    }

    public void ConcluirSeCompleto(ContextoDocumento c, Guid usuarioId)
    {
        if (!c.Assinou(PapelAssinatura.Engenheiro) || !c.Assinou(PapelAssinatura.Proprietario))
            return;

        c.Relato!.Status = StatusRegistro.Assinado; // a partir daqui, imutável
        c.Pendente = false;
    }

    public Task NotificarAsync(ContextoDocumento c, PapelAssinatura papel, CancellationToken ct)
    {
        if (papel == PapelAssinatura.Engenheiro && c.Pendente)
            _notificacoes.EngenheiroAssinou(c.Proprietario, c.Profissional, c.Obra, DocumentoParaAssinar.RelatoVisita, $"/obras/{c.Obra.Id}/registros/{c.Relato!.Id}");
        return Task.CompletedTask;
    }
}
