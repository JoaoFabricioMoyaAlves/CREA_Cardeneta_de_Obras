using System.Text.Json;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Documentos;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadernetaObras.Tests.Application;

public class AssinarDocumentoUseCaseTests
{
    private const string SenhaCerta = "senha-certa";

    private readonly Usuario _profissional = new() { Id = Guid.NewGuid(), Nome = "Ana", Cpf = "111.111.111-11", SenhaHash = "h", Perfil = PerfilUsuario.Engenheiro, TituloProfissional = "Técnico", NumeroRegistro = "CFT 1" };
    private readonly Usuario _proprietario = new() { Id = Guid.NewGuid(), Nome = "Bia", Cpf = "222.222.222-22", SenhaHash = "h", Perfil = PerfilUsuario.Proprietario };
    private readonly Usuario _admin = new() { Id = Guid.NewGuid(), Nome = "Caio", Cpf = "333.333.333-33", SenhaHash = "h", Perfil = PerfilUsuario.Administrador };
    private readonly Usuario _estranho = new() { Id = Guid.NewGuid(), Nome = "Duda", Cpf = "444.444.444-44", SenhaHash = "h", Perfil = PerfilUsuario.Proprietario };

    private readonly Obra _obra;
    private readonly Mock<IObraRepository> _obras = new();
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IAuditLogger> _audit = new();
    private readonly Mock<ITimestampAuthorityService> _tsa = new();
    private readonly Mock<IHashService> _hash = new();
    private readonly Mock<INotificacaoService> _notificacoes = new();
    private readonly Mock<IDocumentoAssinavelRepository> _documentos = new();
    private readonly LimiteTentativasAssinatura _limite = new();
    private DocumentoAssinavel? _congelado;

    public AssinarDocumentoUseCaseTests()
    {
        _obra = new Obra
        {
            Id = 7, NumeroCaderneta = "CAD-2026-0007", IdProfissional = _profissional.Id, IdProprietario = _proprietario.Id,
            LocalObra = "Rua A", Cidade = "Adamantina", TipoEdificacao = "Comercial", NumeroRt = "RT",
        };
        _obras.Setup(r => r.ObterComAssinaturasAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(_obra);
        foreach (var u in new[] { _profissional, _proprietario, _admin, _estranho })
            _usuarios.Setup(r => r.ObterPorIdAsync(u.Id, It.IsAny<CancellationToken>())).ReturnsAsync(u);
        _usuarios.Setup(r => r.ListarPorPerfilAsync(PerfilUsuario.Administrador, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Usuario>());
        _hasher.Setup(h => h.Verificar(It.IsAny<string>(), "h")).Returns<string, string>((s, _) => s == SenhaCerta);
        // Hash de teste legível: o próprio texto em hexadecimal curto e estável.
        _hash.Setup(h => h.GerarHashSha256(It.IsAny<string>()))
            .Returns<string>(s => Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(s))));
        _currentUser.Setup(c => c.Ip).Returns("10.0.0.1");
        _currentUser.Setup(c => c.UserAgent).Returns("ua");
        _documentos.Setup(r => r.ObterAsync(TipoDocumento.AberturaObra, 7, It.IsAny<CancellationToken>())).ReturnsAsync(() => _congelado);
        _documentos.Setup(r => r.Adicionar(It.IsAny<DocumentoAssinavel>())).Callback<DocumentoAssinavel>(d => _congelado = d);
    }

    private void LogarComo(Usuario usuario)
    {
        _currentUser.Setup(c => c.UsuarioId).Returns(usuario.Id);
        _currentUser.Setup(c => c.Perfil).Returns(usuario.Perfil);
    }

    private IEnumerable<IDocumentoHandler> Handlers() =>
        new IDocumentoHandler[] { new AberturaObraHandler(_obras.Object, _usuarios.Object, _audit.Object, _notificacoes.Object) };

    private AssinarDocumentoUseCase Assinar() => new(
        Handlers(), _documentos.Object, _usuarios.Object, _unitOfWork.Object, _hasher.Object,
        _currentUser.Object, _audit.Object, _tsa.Object, _hash.Object, _limite);

    private ObterDocumentoUseCase Obter() => new(Handlers(), _documentos.Object, _usuarios.Object, _currentUser.Object, _hash.Object);

    private async Task<string> HashExibido() =>
        (await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7)).HashDocumento;

    private Task<DocumentoResponse> AssinarComo(Usuario usuario, string hash, string? senha = null, string? cpf = null) =>
        Assinar().ExecutarAsync(TipoDocumento.AberturaObra, 7, new AssinarDocumentoRequest(cpf ?? usuario.Cpf, senha ?? SenhaCerta, hash));

    [Fact]
    public async Task SenhaErrada_Recusa400_RegistraAuditoria_NaoAssina()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();

        await Assert.ThrowsAsync<ValidationAppException>(() => AssinarComo(_profissional, hash, senha: "errada"));

        Assert.Empty(_obra.Assinaturas);
        _audit.Verify(a => a.Registrar("AssinaturaConfirmacaoFalhou", _profissional.Id, "AberturaObra", "7", It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task CpfDeOutroUsuario_Recusa()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();

        await Assert.ThrowsAsync<ValidationAppException>(() => AssinarComo(_profissional, hash, cpf: _admin.Cpf));

        Assert.Empty(_obra.Assinaturas);
    }

    [Fact]
    public async Task Cpf_SemPontuacao_AindaConfere()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();

        await AssinarComo(_profissional, hash, cpf: "11111111111");

        Assert.Single(_obra.Assinaturas);
    }

    [Fact]
    public async Task SeisTentativas_BloqueiaAteComSenhaCerta()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();
        for (var i = 0; i < 5; i++)
            await Assert.ThrowsAsync<ValidationAppException>(() => AssinarComo(_profissional, hash, senha: "errada"));

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() => AssinarComo(_profissional, hash));

        Assert.Contains("15 minutos", ex.Message);
        Assert.Empty(_obra.Assinaturas);
    }

    [Fact]
    public async Task HashDiferente_LancaDocumentoAlterado()
    {
        LogarComo(_profissional);

        await Assert.ThrowsAsync<DocumentoAlteradoException>(() => AssinarComo(_profissional, "outro-hash"));

        Assert.Empty(_obra.Assinaturas);
        Assert.Null(_congelado);
    }

    [Fact]
    public async Task PrimeiraAssinatura_Congela()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();

        var resposta = await AssinarComo(_profissional, hash);

        Assert.NotNull(_congelado);
        Assert.Equal(hash, _congelado!.HashDocumento);
        Assert.Equal(1, _congelado.ModeloVersao);
        Assert.True(resposta.Congelado);
        Assert.Equal(hash, _obra.Assinaturas.Single().CodHash);
    }

    [Fact]
    public async Task SegundaAssinatura_UsaCongelado_MesmoComCadastroAlterado()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();
        await AssinarComo(_profissional, hash);

        _profissional.NumeroRegistro = "CFT mudou";
        LogarComo(_admin);
        var exibido = await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7);
        var resposta = await AssinarComo(_admin, exibido.HashDocumento);

        Assert.Equal(hash, exibido.HashDocumento);
        Assert.Equal("CFT 1", resposta.Dados.GetProperty("profissional").GetProperty("registro").GetString());
        _documentos.Verify(r => r.Adicionar(It.IsAny<DocumentoAssinavel>()), Times.Once);
        Assert.Equal(StatusObra.Ativa, _obra.Status);
    }

    [Fact]
    public async Task HashAssinatura_DifereEntreSignatarios()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();
        await AssinarComo(_profissional, hash);
        LogarComo(_admin);
        await AssinarComo(_admin, hash);

        var hashes = _obra.Assinaturas.Select(a => a.HashAssinatura).ToList();
        Assert.Equal(2, hashes.Distinct().Count());
        Assert.All(hashes, h => Assert.Equal(64, h!.Length));
    }

    [Fact]
    public async Task AssinaturaDuplicada_Recusa()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();
        await AssinarComo(_profissional, hash);

        await Assert.ThrowsAsync<AssinaturaDuplicadaException>(() => AssinarComo(_profissional, hash));
    }

    [Fact]
    public async Task ObraJaAtiva_Recusa()
    {
        _obra.Status = StatusObra.Ativa;
        LogarComo(_profissional);
        var hash = await HashExibido();

        await Assert.ThrowsAsync<EntidadeImutavelException>(() => AssinarComo(_profissional, hash));
    }

    [Fact]
    public async Task CarimboIndisponivel_AindaAssina()
    {
        _tsa.Setup(t => t.ObterCarimboAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync((CarimboTempo?)null);
        LogarComo(_profissional);
        var hash = await HashExibido();

        await AssinarComo(_profissional, hash);

        Assert.Null(_obra.Assinaturas.Single().TsaToken);
    }

    [Fact]
    public async Task CorridaNoCongelamento_MensagemTenteNovamente()
    {
        LogarComo(_profissional);
        var hash = await HashExibido();
        _unitOfWork.Setup(u => u.SalvarAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new ConflitoConcorrenciaException());

        var ex = await Assert.ThrowsAsync<ConflitoConcorrenciaException>(() => AssinarComo(_profissional, hash));

        Assert.Contains("tente novamente", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Obter_ComAssinaturaSemCongelado_MarcaAnteriorAoModelo()
    {
        _obra.Assinaturas.Add(new AssinaturaObra { UsuarioId = _profissional.Id, Papel = PapelAssinatura.Engenheiro, CodHash = "antigo", Ip = "1.1.1.1", UserAgent = "ua" });
        LogarComo(_admin);

        var resposta = await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7);

        Assert.True(resposta.AnteriorAoModelo);
        Assert.False(resposta.Congelado);
        Assert.Equal("antigo", resposta.Assinaturas.Single().Hash);
        Assert.Equal("Ana", resposta.Assinaturas.Single().Nome);
    }

    [Fact]
    public async Task Obter_ObraJaAtiva_InformaQueNaoEstaPendente()
    {
        _obra.Status = StatusObra.Ativa;
        LogarComo(_admin);

        var resposta = await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7);

        Assert.False(resposta.Pendente);
    }

    [Fact]
    public async Task Obter_ObraPendente_InformaQueEstaPendente()
    {
        LogarComo(_admin);

        var resposta = await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7);

        Assert.True(resposta.Pendente);
    }

    [Fact]
    public async Task Obter_UsuarioSemRelacao_403()
    {
        LogarComo(_estranho);

        await Assert.ThrowsAsync<ForbiddenException>(() => Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7));
    }

    [Fact]
    public async Task Obter_InformaQuemPodeAssinar()
    {
        LogarComo(_proprietario);
        Assert.Null((await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7)).PapelQuePodeAssinar);

        LogarComo(_admin);
        Assert.Null((await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7)).PapelQuePodeAssinar);

        LogarComo(_profissional);
        Assert.Equal("Engenheiro", (await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7)).PapelQuePodeAssinar);
    }

    [Fact]
    public async Task Dados_SaoJsonDoDocumento()
    {
        LogarComo(_profissional);

        var resposta = await Obter().ExecutarAsync(TipoDocumento.AberturaObra, 7);

        Assert.Equal(JsonValueKind.Object, resposta.Dados.ValueKind);
        Assert.Equal("CAD-2026-0007", resposta.Dados.GetProperty("numeroCaderneta").GetString());
    }
}
