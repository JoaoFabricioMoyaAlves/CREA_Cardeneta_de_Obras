using CadernetaObras.Application.Assinaturas;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Exceptions;
using CadernetaObras.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadernetaObras.Tests.Application;

public class AssinarObraUseCaseTests
{
    private readonly Guid _engenheiroId = Guid.NewGuid();
    private readonly Guid _proprietarioId = Guid.NewGuid();
    private readonly Mock<IObraRepository> _obraRepo = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IHashService> _hashService = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IAuditLogger> _auditLogger = new();
    private readonly Mock<ITimestampAuthorityService> _tsa = new();

    private Obra CriarObraPendente() => new()
    {
        Id = 1,
        NumeroCaderneta = "CAD-2026-0001",
        IdProfissional = _engenheiroId,
        IdProprietario = _proprietarioId,
        Status = StatusObra.PendenteAssinatura,
        Assinaturas = new List<AssinaturaObra>(),
    };

    private const string HashFixo = "deadbeef00112233445566778899aabbccddeeff0011223344556677889900";

    private AssinarObraUseCase CriarUseCase()
    {
        _hashService.Setup(h => h.GerarHashSha256(It.IsAny<string>())).Returns(HashFixo);
        _tsa.Setup(t => t.ObterCarimboAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarimboTempo?)null); // TSA desligada nos testes — comportamento best-effort
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(_engenheiroId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = _engenheiroId, Nome = "Eng. Teste", Perfil = PerfilUsuario.Engenheiro });
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(_proprietarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = _proprietarioId, Nome = "Prop. Teste", Perfil = PerfilUsuario.Proprietario });

        return new AssinarObraUseCase(
            _obraRepo.Object, _usuarioRepo.Object, _unitOfWork.Object, _hashService.Object,
            _currentUser.Object, _auditLogger.Object, _tsa.Object);
    }

    [Fact]
    public async Task Assinar_PrimeiraAssinatura_NaoAtivaObraAinda()
    {
        var obra = CriarObraPendente();
        _obraRepo.Setup(r => r.ObterComAssinaturasAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        _currentUser.Setup(c => c.UsuarioId).Returns(_engenheiroId);
        _currentUser.Setup(c => c.Perfil).Returns(PerfilUsuario.Engenheiro);
        _currentUser.Setup(c => c.Ip).Returns("127.0.0.1");
        _currentUser.Setup(c => c.UserAgent).Returns("xunit-test");

        var resultado = await CriarUseCase().ExecutarAsync(1);

        Assert.Equal("PendenteAssinatura", resultado.Status);
        var assinatura = Assert.Single(obra.Assinaturas);
        Assert.Equal(PapelAssinatura.Engenheiro, assinatura.Papel);
        Assert.Equal("127.0.0.1", assinatura.Ip);
        Assert.Equal(HashFixo, assinatura.CodHash);
    }

    [Fact]
    public async Task Assinar_AmbosOsPapeisAssinaram_ObraFicaAtiva()
    {
        var obra = CriarObraPendente();
        obra.Assinaturas.Add(new AssinaturaObra
        {
            ObraId = 1, UsuarioId = _proprietarioId, Papel = PapelAssinatura.Proprietario,
            Data = DateTime.UtcNow, CodHash = "outro-hash", Ip = "1.1.1.1", UserAgent = "outro",
        });
        _obraRepo.Setup(r => r.ObterComAssinaturasAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        _currentUser.Setup(c => c.UsuarioId).Returns(_engenheiroId);
        _currentUser.Setup(c => c.Perfil).Returns(PerfilUsuario.Engenheiro);
        _currentUser.Setup(c => c.Ip).Returns("127.0.0.1");
        _currentUser.Setup(c => c.UserAgent).Returns("xunit-test");

        var resultado = await CriarUseCase().ExecutarAsync(1);

        Assert.Equal("Ativa", resultado.Status);
        Assert.Equal(2, obra.Assinaturas.Count);
    }

    [Fact]
    public async Task Assinar_UsuarioNaoAtribuidoAObra_LancaForbidden()
    {
        var obra = CriarObraPendente();
        _obraRepo.Setup(r => r.ObterComAssinaturasAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        _currentUser.Setup(c => c.UsuarioId).Returns(Guid.NewGuid()); // usuário aleatório, não é nem engenheiro nem proprietário desta obra
        _currentUser.Setup(c => c.Perfil).Returns(PerfilUsuario.Engenheiro);

        await Assert.ThrowsAsync<ForbiddenException>(() => CriarUseCase().ExecutarAsync(1));
    }

    [Fact]
    public async Task Assinar_MesmoUsuarioAssinaDuasVezes_LancaAssinaturaDuplicada()
    {
        var obra = CriarObraPendente();
        obra.Assinaturas.Add(new AssinaturaObra
        {
            ObraId = 1, UsuarioId = _engenheiroId, Papel = PapelAssinatura.Engenheiro,
            Data = DateTime.UtcNow, CodHash = "hash-anterior", Ip = "1.1.1.1", UserAgent = "outro",
        });
        _obraRepo.Setup(r => r.ObterComAssinaturasAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(obra);
        _currentUser.Setup(c => c.UsuarioId).Returns(_engenheiroId);
        _currentUser.Setup(c => c.Perfil).Returns(PerfilUsuario.Engenheiro);

        await Assert.ThrowsAsync<AssinaturaDuplicadaException>(() => CriarUseCase().ExecutarAsync(1));
    }

    [Fact]
    public async Task Assinar_ObraJaAtiva_LancaEntidadeImutavel()
    {
        var obra = CriarObraPendente();
        obra.Status = StatusObra.Ativa;
        _obraRepo.Setup(r => r.ObterComAssinaturasAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(obra);

        await Assert.ThrowsAsync<EntidadeImutavelException>(() => CriarUseCase().ExecutarAsync(1));
    }
}
