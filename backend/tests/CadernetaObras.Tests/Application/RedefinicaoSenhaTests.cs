using CadernetaObras.Application.Auth;
using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Notificacoes;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Moq;
using Xunit;

namespace CadernetaObras.Tests.Application;

public class RedefinicaoSenhaTests
{
    private readonly Usuario _usuario = new() { Id = Guid.NewGuid(), Nome = "Fulano", Email = "fulano@teste.com", SenhaHash = "hash-antigo" };
    private readonly Mock<IUsuarioRepository> _usuarioRepo = new();
    private readonly Mock<ITokenSenhaRepository> _tokenRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IAuditLogger> _auditLogger = new();
    private readonly Mock<INotificacaoService> _notificacoes = new();

    public RedefinicaoSenhaTests()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_usuario);
        _usuarioRepo.Setup(r => r.ObterPorEmailAsync("fulano@teste.com", It.IsAny<CancellationToken>())).ReturnsAsync(_usuario);
        _tokenRepo.Setup(r => r.ListarPendentesDoUsuarioAsync(_usuario.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _passwordHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns<string>(s => $"hash:{s}");
    }

    private RedefinirSenhaUseCase Redefinir() => new(
        _usuarioRepo.Object, _tokenRepo.Object, _unitOfWork.Object, _passwordHasher.Object, _auditLogger.Object);

    private SolicitarRecuperacaoSenhaUseCase Solicitar() => new(
        _usuarioRepo.Object, _tokenRepo.Object, _unitOfWork.Object, _notificacoes.Object, _auditLogger.Object);

    private string TokenNoBanco(Action<TokenSenha>? ajustar = null)
    {
        var (entidade, token) = TokenSenhaGerador.Criar(_usuario.Id, FinalidadeTokenSenha.RecuperacaoSenha);
        ajustar?.Invoke(entidade);
        _tokenRepo.Setup(r => r.ObterPorHashAsync(entidade.TokenHash, It.IsAny<CancellationToken>())).ReturnsAsync(entidade);
        return token;
    }

    [Fact]
    public async Task Redefinir_TokenValido_TrocaSenhaEInutilizaToken()
    {
        TokenSenha? entidade = null;
        var token = TokenNoBanco(t => entidade = t);

        await Redefinir().ExecutarAsync(new RedefinirSenhaRequest(token, "NovaSenha123"));

        Assert.Equal("hash:NovaSenha123", _usuario.SenhaHash);
        Assert.NotNull(entidade!.UsadoEm);
    }

    [Fact]
    public async Task Redefinir_TokenExpirado_Recusa()
    {
        var token = TokenNoBanco(t => t.ExpiraEm = DateTime.UtcNow.AddMinutes(-1));

        await Assert.ThrowsAsync<ValidationAppException>(() => Redefinir().ExecutarAsync(new RedefinirSenhaRequest(token, "NovaSenha123")));
        Assert.Equal("hash-antigo", _usuario.SenhaHash);
    }

    [Fact]
    public async Task Redefinir_TokenJaUsado_Recusa()
    {
        var token = TokenNoBanco(t => t.UsadoEm = DateTime.UtcNow.AddMinutes(-5));

        await Assert.ThrowsAsync<ValidationAppException>(() => Redefinir().ExecutarAsync(new RedefinirSenhaRequest(token, "NovaSenha123")));
    }

    [Fact]
    public async Task Redefinir_TokenInexistente_Recusa()
    {
        await Assert.ThrowsAsync<ValidationAppException>(() => Redefinir().ExecutarAsync(new RedefinirSenhaRequest("token-qualquer", "NovaSenha123")));
    }

    [Fact]
    public async Task Redefinir_SenhaCurta_Recusa()
    {
        var token = TokenNoBanco();

        await Assert.ThrowsAsync<ValidationAppException>(() => Redefinir().ExecutarAsync(new RedefinirSenhaRequest(token, "1234567")));
    }

    [Fact]
    public async Task Solicitar_EmailCadastrado_EnviaLinkEInvalidaLinksAnteriores()
    {
        var anterior = new TokenSenha { Id = Guid.NewGuid(), UsuarioId = _usuario.Id, ExpiraEm = DateTime.UtcNow.AddMinutes(30) };
        _tokenRepo.Setup(r => r.ListarPendentesDoUsuarioAsync(_usuario.Id, It.IsAny<CancellationToken>())).ReturnsAsync([anterior]);

        await Solicitar().ExecutarAsync(new SolicitarRecuperacaoSenhaRequest("fulano@teste.com"));

        Assert.NotNull(anterior.UsadoEm);
        _tokenRepo.Verify(r => r.Adicionar(It.Is<TokenSenha>(t => t.Finalidade == FinalidadeTokenSenha.RecuperacaoSenha)), Times.Once);
        _notificacoes.Verify(n => n.RecuperacaoSenha(_usuario, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Solicitar_EmailDesconhecido_NaoFalhaENaoEnvia()
    {
        await Solicitar().ExecutarAsync(new SolicitarRecuperacaoSenhaRequest("ninguem@teste.com"));

        _notificacoes.Verify(n => n.RecuperacaoSenha(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
        _tokenRepo.Verify(r => r.Adicionar(It.IsAny<TokenSenha>()), Times.Never);
    }
}
