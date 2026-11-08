using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Documentos;

public class TermoConclusaoHandler : IDocumentoHandler
{
    private readonly ITermoConclusaoRepository _termos;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly IAuditLogger _auditLogger;
    private readonly INotificacaoService _notificacoes;

    public TermoConclusaoHandler(
        ITermoConclusaoRepository termos, IObraRepository obras, IUsuarioRepository usuarios,
        IAuditLogger auditLogger, INotificacaoService notificacoes)
    {
        _termos = termos;
        _obras = obras;
        _usuarios = usuarios;
        _auditLogger = auditLogger;
        _notificacoes = notificacoes;
    }

    public TipoDocumento Tipo => TipoDocumento.TermoConclusao;
    public string NomeEntidade => "Termo de conclusão";

    public async Task<ContextoDocumento> CarregarAsync(int referenciaId, CancellationToken ct)
    {
        var termo = await _termos.ObterComAssinaturasAsync(referenciaId, ct)
            ?? throw new NotFoundException("Termo de conclusão não encontrado.");
        var obra = await _obras.ObterPorIdAsync(termo.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        return new ContextoDocumento
        {
            Obra = obra,
            Termo = termo,
            Profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct)
                ?? throw new NotFoundException("Responsável técnico da obra não encontrado."),
            Proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct)
                ?? throw new NotFoundException("Proprietário da obra não encontrado."),
            Pendente = termo.Status == StatusRegistro.PendenteAssinatura,
            Assinaturas = termo.Assinaturas
                .Select(a => new AssinaturaInfo(a.Papel, a.UsuarioId, a.Data, a.CodHash, a.HashAssinatura, a.Ip, a.TsaToken is not null, a.TsaAutoridade, a.TsaDataHora))
                .ToList(),
        };
    }

    public Task<string> MontarDadosJsonAsync(ContextoDocumento c, CancellationToken ct) =>
        Task.FromResult(DocumentoJson.Serializar(new DadosTermo(
            c.Obra.NumeroCaderneta,
            DocumentoHandlerHelpers.Profissional(c.Profissional),
            DocumentoHandlerHelpers.Empresa(c.Obra),
            DocumentoHandlerHelpers.Proprietario(c.Proprietario),
            c.Obra.LocalObra, c.Obra.Cidade, c.Obra.Estado,
            c.Termo!.DataConclusao,
            c.Termo.Declaracao)));

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
        c.Termo!.Assinaturas.Add(new AssinaturaTermoConclusao
        {
            TermoConclusaoId = c.Termo.Id, UsuarioId = n.UsuarioId, Papel = n.Papel, Data = n.Data,
            CodHash = n.CodHash, HashAssinatura = n.HashAssinatura, Ip = n.Ip, UserAgent = n.UserAgent,
            TsaToken = n.Carimbo?.TokenBase64, TsaDataHora = n.Carimbo?.DataHoraTsa, TsaAutoridade = n.Carimbo?.Autoridade,
        });
        c.Assinaturas.Add(new AssinaturaInfo(n.Papel, n.UsuarioId, n.Data, n.CodHash, n.HashAssinatura, n.Ip, n.Carimbo is not null, n.Carimbo?.Autoridade, n.Carimbo?.DataHoraTsa));
        _auditLogger.Registrar("TermoConclusaoAssinado", n.UsuarioId, "TermoConclusao", c.Termo.Id.ToString(), $"Papel: {n.Papel}");
    }

    public void ConcluirSeCompleto(ContextoDocumento c, Guid usuarioId)
    {
        if (!c.Assinou(PapelAssinatura.Engenheiro) || !c.Assinou(PapelAssinatura.Proprietario))
            return;

        c.Termo!.Status = StatusRegistro.Assinado;
        c.Obra.Status = StatusObra.Finalizada; // encerra a caderneta por completo
        c.Pendente = false;
        _auditLogger.Registrar("ObraFinalizada", usuarioId, "Obra", c.Obra.Id.ToString());
    }

    public Task NotificarAsync(ContextoDocumento c, PapelAssinatura papel, CancellationToken ct)
    {
        if (papel == PapelAssinatura.Engenheiro && c.Pendente)
            _notificacoes.EngenheiroAssinou(c.Proprietario, c.Profissional, c.Obra, DocumentoParaAssinar.TermoConclusao, $"/obras/{c.Obra.Id}");
        return Task.CompletedTask;
    }
}
