using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Application.Obras;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Documentos;

// A abertura é assinada pelo Eng/Arq/Téc responsável e depois por um
// Administrador do CREA. Assinaturas antigas de proprietário continuam no
// documento, mas não contam mais para ativar a obra.
public class AberturaObraHandler : IDocumentoHandler
{
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IAuditLogger _auditLogger;
    private readonly INotificacaoService _notificacoes;

    public AberturaObraHandler(IObraRepository obras, IUsuarioRepository usuarios, IAuditLogger auditLogger, INotificacaoService notificacoes)
    {
        _obras = obras;
        _usuarios = usuarios;
        _auditLogger = auditLogger;
        _notificacoes = notificacoes;
    }

    public TipoDocumento Tipo => TipoDocumento.AberturaObra;
    public string NomeEntidade => "Obra";

    public async Task<ContextoDocumento> CarregarAsync(int referenciaId, CancellationToken ct)
    {
        var obra = await _obras.ObterComAssinaturasAsync(referenciaId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        return new ContextoDocumento
        {
            Obra = obra,
            Profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct)
                ?? throw new NotFoundException("Responsável técnico da obra não encontrado."),
            Proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct)
                ?? throw new NotFoundException("Proprietário da obra não encontrado."),
            Pendente = obra.Status == StatusObra.PendenteAssinatura,
            Assinaturas = obra.Assinaturas
                .Select(a => new AssinaturaInfo(a.Papel, a.UsuarioId, a.Data, a.CodHash, a.HashAssinatura, a.Ip, a.TsaToken is not null, a.TsaAutoridade, a.TsaDataHora))
                .ToList(),
        };
    }

    public Task<string> MontarDadosJsonAsync(ContextoDocumento c, CancellationToken ct)
    {
        var o = c.Obra;
        return Task.FromResult(DocumentoJson.Serializar(new DadosAbertura(
            o.NumeroCaderneta,
            DocumentoHandlerHelpers.Profissional(c.Profissional),
            DocumentoHandlerHelpers.Empresa(o),
            DocumentoHandlerHelpers.Proprietario(c.Proprietario),
            o.LocalObra, o.Cidade, o.Estado,
            o.AreaConstruirM2, o.AreaAmpliarM2, o.AreaReformarM2, o.AreaRegularizarM2, o.AreaTotalEdificadaM2,
            o.TipoEdificacao, o.TipoEdificacaoOutros,
            ObraMapper.ListarAtividades(o),
            o.NumeroRt, o.ValorObra, o.DataReciboAbertura)));
    }

    public PapelAssinatura DeterminarPapel(ContextoDocumento c, Guid usuarioId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Engenheiro && c.Obra.IdProfissional == usuarioId)
            return PapelAssinatura.Engenheiro;

        if (perfil == PerfilUsuario.Administrador)
        {
            if (!c.Assinou(PapelAssinatura.Engenheiro))
                throw new ForbiddenException("O responsável técnico precisa assinar antes do Administrador.");
            return PapelAssinatura.Administrador;
        }

        throw new ForbiddenException("Somente o responsável técnico da obra e o Administrador do CREA assinam a abertura.");
    }

    public void AdicionarAssinatura(ContextoDocumento c, NovaAssinatura n)
    {
        c.Obra.Assinaturas.Add(new AssinaturaObra
        {
            ObraId = c.Obra.Id, UsuarioId = n.UsuarioId, Papel = n.Papel, Data = n.Data,
            CodHash = n.CodHash, HashAssinatura = n.HashAssinatura, Ip = n.Ip, UserAgent = n.UserAgent,
            TsaToken = n.Carimbo?.TokenBase64, TsaDataHora = n.Carimbo?.DataHoraTsa, TsaAutoridade = n.Carimbo?.Autoridade,
        });
        c.Assinaturas.Add(new AssinaturaInfo(n.Papel, n.UsuarioId, n.Data, n.CodHash, n.HashAssinatura, n.Ip, n.Carimbo is not null, n.Carimbo?.Autoridade, n.Carimbo?.DataHoraTsa));
        _auditLogger.Registrar("ObraAssinada", n.UsuarioId, "Obra", c.Obra.Id.ToString(), $"Papel: {n.Papel}");
    }

    public void ConcluirSeCompleto(ContextoDocumento c, Guid usuarioId)
    {
        if (!c.Assinou(PapelAssinatura.Engenheiro) || !c.Assinou(PapelAssinatura.Administrador))
            return;

        c.Obra.IdAdministrador = c.Assinaturas.First(a => a.Papel == PapelAssinatura.Administrador).UsuarioId;
        c.Obra.Status = StatusObra.Ativa;
        c.Pendente = false;
        _auditLogger.Registrar("ObraAtivada", usuarioId, "Obra", c.Obra.Id.ToString());
    }

    public async Task NotificarAsync(ContextoDocumento c, PapelAssinatura papel, CancellationToken ct)
    {
        if (papel != PapelAssinatura.Engenheiro || !c.Pendente)
            return;

        foreach (var admin in await _usuarios.ListarPorPerfilAsync(PerfilUsuario.Administrador, ct))
            _notificacoes.AberturaAguardandoAdministrador(admin, c.Profissional, c.Obra, $"/obras/{c.Obra.Id}");
    }
}
