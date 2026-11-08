using System.Security.Cryptography;
using System.Text;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Documentos;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Application.Obras;
using CadernetaObras.Application.Registros;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadernetaObras.Tests.Application;

public class DocumentoHandlersTests
{
    private readonly Usuario _profissional = new()
    {
        Id = Guid.NewGuid(), Nome = "Téc. Ana", Cpf = "111.111.111-11", Telefone = "(18) 1111-1111",
        Perfil = PerfilUsuario.Engenheiro, TituloProfissional = "Técnico", NumeroRegistro = "CFT 123",
    };
    private readonly Usuario _proprietario = new()
    {
        Id = Guid.NewGuid(), Nome = "Bia", Cpf = "222.222.222-22", Telefone = "(18) 2222-2222",
        Perfil = PerfilUsuario.Proprietario,
    };
    private readonly Usuario _admin = new()
    {
        Id = Guid.NewGuid(), Nome = "Caio", Cpf = "333.333.333-33", Perfil = PerfilUsuario.Administrador,
    };

    private readonly Mock<IObraRepository> _obras = new();
    private readonly Mock<IRelatoVisitaRepository> _relatos = new();
    private readonly Mock<ITermoConclusaoRepository> _termos = new();
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IStorageService> _storage = new();
    private readonly Mock<IAuditLogger> _audit = new();
    private readonly Mock<INotificacaoService> _notificacoes = new();

    private readonly Obra _obra;

    public DocumentoHandlersTests()
    {
        _obra = new Obra
        {
            Id = 7, NumeroCaderneta = "CAD-2026-0007", IdProfissional = _profissional.Id, IdProprietario = _proprietario.Id,
            LocalObra = "Rua A, 10", Cidade = "Adamantina", Estado = "SP", NumeroRt = "RT-1",
            AreaConstruirM2 = 10, TipoEdificacao = "Comercial", ValorObra = 1000,
            DataReciboAbertura = new DateOnly(2026, 11, 8), AtivTecnicaExecucao = true,
        };
        _obras.Setup(r => r.ObterComAssinaturasAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(_obra);
        _obras.Setup(r => r.ObterPorIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(_obra);
        foreach (var u in new[] { _profissional, _proprietario, _admin })
            _usuarios.Setup(r => r.ObterPorIdAsync(u.Id, It.IsAny<CancellationToken>())).ReturnsAsync(u);
        _usuarios.Setup(r => r.ListarPorPerfilAsync(PerfilUsuario.Administrador, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Usuario> { _admin });
    }

    private AberturaObraHandler Abertura() => new(_obras.Object, _usuarios.Object, _audit.Object, _notificacoes.Object);
    private RelatoVisitaHandler Relato() => new(_relatos.Object, _obras.Object, _usuarios.Object, _storage.Object, _audit.Object, _notificacoes.Object);
    private TermoConclusaoHandler Termo() => new(_termos.Object, _obras.Object, _usuarios.Object, _audit.Object, _notificacoes.Object);

    private static NovaAssinatura Nova(Guid usuarioId, PapelAssinatura papel) =>
        new(usuarioId, papel, DateTime.UtcNow, "hashdoc", "hashass", "10.0.0.1", "ua", null);

    private void AssinaturaObra(PapelAssinatura papel, Guid usuarioId) =>
        _obra.Assinaturas.Add(new AssinaturaObra { ObraId = 7, UsuarioId = usuarioId, Papel = papel, CodHash = "x", Ip = "1.1.1.1", UserAgent = "ua" });

    [Fact]
    public async Task Abertura_Admin_SoAposProfissional()
    {
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        Assert.Throws<ForbiddenException>(() =>
            handler.DeterminarPapel(contexto, _admin.Id, PerfilUsuario.Administrador));
    }

    [Fact]
    public async Task Abertura_Admin_AposProfissional_Assina()
    {
        AssinaturaObra(PapelAssinatura.Engenheiro, _profissional.Id);
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        Assert.Equal(PapelAssinatura.Administrador, handler.DeterminarPapel(contexto, _admin.Id, PerfilUsuario.Administrador));
    }

    [Fact]
    public async Task Abertura_Proprietario_NaoAssina()
    {
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        Assert.Throws<ForbiddenException>(() =>
            handler.DeterminarPapel(contexto, _proprietario.Id, PerfilUsuario.Proprietario));
    }

    [Fact]
    public async Task Abertura_CompletaComProfissionalEAdmin_AtivaObraEGravaIdAdministrador()
    {
        AssinaturaObra(PapelAssinatura.Engenheiro, _profissional.Id);
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        handler.AdicionarAssinatura(contexto, Nova(_admin.Id, PapelAssinatura.Administrador));
        handler.ConcluirSeCompleto(contexto, _admin.Id);

        Assert.Equal(StatusObra.Ativa, _obra.Status);
        Assert.Equal(_admin.Id, _obra.IdAdministrador);
        Assert.Equal("hashass", _obra.Assinaturas.Single(a => a.Papel == PapelAssinatura.Administrador).HashAssinatura);
    }

    [Fact]
    public async Task Abertura_ComAssinaturaAntigaDeProprietario_AindaExigeAdmin()
    {
        AssinaturaObra(PapelAssinatura.Proprietario, _proprietario.Id);
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        handler.AdicionarAssinatura(contexto, Nova(_profissional.Id, PapelAssinatura.Engenheiro));
        handler.ConcluirSeCompleto(contexto, _profissional.Id);

        Assert.Equal(StatusObra.PendenteAssinatura, _obra.Status);
    }

    [Fact]
    public async Task Abertura_ProfissionalAssinou_AvisaTodosOsAdministradores()
    {
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        await handler.NotificarAsync(contexto, PapelAssinatura.Engenheiro, default);

        _notificacoes.Verify(n => n.AberturaAguardandoAdministrador(_admin, _profissional, _obra, "/obras/7"), Times.Once);
    }

    [Fact]
    public async Task Abertura_Dados_TrazemCamposDoDocumento()
    {
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        var json = await handler.MontarDadosJsonAsync(contexto, default);

        Assert.Contains("\"numeroCaderneta\":\"CAD-2026-0007\"", json);
        Assert.Contains("\"registro\":\"CFT 123\"", json);
        Assert.Contains("\"telefone\":\"(18) 2222-2222\"", json);
        Assert.Contains("\"estado\":\"SP\"", json);
        Assert.Contains("Execu", json);
    }

    [Fact]
    public async Task MontarDadosJson_MesmaEntrada_MesmoTexto()
    {
        var handler = Abertura();
        var contexto = await handler.CarregarAsync(7, default);

        Assert.Equal(await handler.MontarDadosJsonAsync(contexto, default), await handler.MontarDadosJsonAsync(contexto, default));
    }

    private RelatoVisita NovoRelato()
    {
        var relato = new RelatoVisita
        {
            Id = 3, ObraId = 7, DataVisita = new DateOnly(2026, 11, 9), PosicaoObra = "Em andamento",
            DecisoesOrientacoes = "Texto", FaseFundacao = true,
        };
        _relatos.Setup(r => r.ObterComAssinaturasAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(relato);
        _relatos.Setup(r => r.ListarPorObraAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RelatoVisita> { relato, new() { Id = 2, ObraId = 7 } });
        return relato;
    }

    [Fact]
    public async Task Relato_CompletoComProfissionalEProprietario_FicaAssinado()
    {
        var relato = NovoRelato();
        var handler = Relato();
        var contexto = await handler.CarregarAsync(3, default);

        handler.AdicionarAssinatura(contexto, Nova(_profissional.Id, PapelAssinatura.Engenheiro));
        handler.ConcluirSeCompleto(contexto, _profissional.Id);
        Assert.Equal(StatusRegistro.PendenteAssinatura, relato.Status);

        handler.AdicionarAssinatura(contexto, Nova(_proprietario.Id, PapelAssinatura.Proprietario));
        handler.ConcluirSeCompleto(contexto, _proprietario.Id);
        Assert.Equal(StatusRegistro.Assinado, relato.Status);
    }

    [Fact]
    public async Task Relato_Admin_NaoAssina()
    {
        NovoRelato();
        var handler = Relato();
        var contexto = await handler.CarregarAsync(3, default);

        Assert.Throws<ForbiddenException>(() => handler.DeterminarPapel(contexto, _admin.Id, PerfilUsuario.Administrador));
    }

    [Fact]
    public async Task Relato_HashDaFotoEntraNosDados()
    {
        var relato = NovoRelato();
        relato.Imagens.Add(new Imagem { Id = 9, RelatoVisitaId = 3, Name = "foto.jpg", StorageKey = "k" });
        var bytes = Encoding.UTF8.GetBytes("conteudo");
        _storage.Setup(s => s.ObterArquivoAsync("k", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(bytes));
        var handler = Relato();
        var contexto = await handler.CarregarAsync(3, default);

        var json = await handler.MontarDadosJsonAsync(contexto, default);

        Assert.Contains(Convert.ToHexStringLower(SHA256.HashData(bytes)), json);
        Assert.Contains("\"sequencia\":2", json); // id 2 é o primeiro relato da obra; id 3, o segundo
        Assert.Contains("Funda", json);
    }

    [Fact]
    public async Task Relato_Pendente_FotoIndisponivel_MensagemClara()
    {
        var relato = NovoRelato();
        relato.Imagens.Add(new Imagem { Id = 9, RelatoVisitaId = 3, Name = "foto.jpg", StorageKey = "k" });
        _storage.Setup(s => s.ObterArquivoAsync("k", It.IsAny<CancellationToken>())).ThrowsAsync(new IOException("sem arquivo"));
        var handler = Relato();
        var contexto = await handler.CarregarAsync(3, default);

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() => handler.MontarDadosJsonAsync(contexto, default));

        Assert.Contains("foto.jpg", ex.Message);
    }

    [Fact]
    public async Task Relato_JaAssinado_FotoIndisponivel_AindaAbre()
    {
        var relato = NovoRelato();
        relato.Status = StatusRegistro.Assinado;
        relato.Imagens.Add(new Imagem { Id = 9, RelatoVisitaId = 3, Name = "foto.jpg", StorageKey = "k" });
        _storage.Setup(s => s.ObterArquivoAsync("k", It.IsAny<CancellationToken>())).ThrowsAsync(new IOException("sem arquivo"));
        var handler = Relato();
        var contexto = await handler.CarregarAsync(3, default);

        var json = await handler.MontarDadosJsonAsync(contexto, default);

        Assert.Contains("\"sha256\":null", json);
    }

    [Fact]
    public async Task Termo_Completo_FinalizaObra()
    {
        var termo = new TermoConclusao { Id = 4, ObraId = 7, DataConclusao = new DateOnly(2026, 11, 10), Declaracao = "Concluída" };
        _termos.Setup(r => r.ObterComAssinaturasAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(termo);
        var handler = Termo();
        var contexto = await handler.CarregarAsync(4, default);

        handler.AdicionarAssinatura(contexto, Nova(_profissional.Id, PapelAssinatura.Engenheiro));
        handler.AdicionarAssinatura(contexto, Nova(_proprietario.Id, PapelAssinatura.Proprietario));
        handler.ConcluirSeCompleto(contexto, _proprietario.Id);

        Assert.Equal(StatusRegistro.Assinado, termo.Status);
        Assert.Equal(StatusObra.Finalizada, _obra.Status);
    }

    [Fact]
    public async Task AdicionarImagem_ComRelatoJaAssinadoPorAlguem_Recusa()
    {
        var relato = NovoRelato();
        relato.Assinaturas.Add(new AssinaturaRelato { UsuarioId = _profissional.Id, Papel = PapelAssinatura.Engenheiro });
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.Perfil).Returns(PerfilUsuario.Engenheiro);
        currentUser.Setup(c => c.UsuarioId).Returns(_profissional.Id);
        var useCase = new AdicionarImagemUseCase(_relatos.Object, _obras.Object,
            new Mock<IUnitOfWork>().Object, _storage.Object, currentUser.Object, _audit.Object);

        await Assert.ThrowsAsync<EntidadeImutavelException>(() => useCase.ExecutarAsync(
            new AdicionarImagemRequest(3, "foto.jpg", "image/jpeg", new MemoryStream())));
    }
}
