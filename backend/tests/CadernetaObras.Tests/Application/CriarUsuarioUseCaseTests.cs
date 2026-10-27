using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Usuarios;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CadernetaObras.Tests.Application;

public class CriarUsuarioUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IAuditLogger> _auditLogger = new();

    private CriarUsuarioUseCase CriarUseCase(PerfilUsuario perfilLogado)
    {
        _currentUser.Setup(c => c.UsuarioId).Returns(Guid.NewGuid());
        _currentUser.Setup(c => c.Perfil).Returns(perfilLogado);
        _passwordHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hash");
        var configuration = new ConfigurationBuilder().Build();

        return new CriarUsuarioUseCase(
            _usuarioRepo.Object, _unitOfWork.Object, _passwordHasher.Object,
            _currentUser.Object, configuration, _auditLogger.Object);
    }

    private static CriarUsuarioRequest Request(PerfilUsuario perfil) => new(
        "Fulano", "123.456.789-00", "fulano@teste.com", "(11) 99999-9999", perfil,
        perfil == PerfilUsuario.Engenheiro ? "Engenheiro Civil" : null,
        perfil == PerfilUsuario.Engenheiro ? "CREA-SP 123" : null,
        null);

    [Fact]
    public async Task Engenheiro_PodeCadastrarProprietario()
    {
        var resultado = await CriarUseCase(PerfilUsuario.Engenheiro).ExecutarAsync(Request(PerfilUsuario.Proprietario));

        Assert.Equal("Proprietario", resultado.Usuario.Perfil);
        _usuarioRepo.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Once);
    }

    [Theory]
    [InlineData(PerfilUsuario.Engenheiro)]
    [InlineData(PerfilUsuario.Administrador)]
    public async Task Engenheiro_NaoPodeCadastrarOutrosPerfis(PerfilUsuario perfil)
    {
        var useCase = CriarUseCase(PerfilUsuario.Engenheiro);

        await Assert.ThrowsAsync<ForbiddenException>(() => useCase.ExecutarAsync(Request(perfil)));
        _usuarioRepo.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Proprietario_NaoPodeCadastrarUsuario()
    {
        var useCase = CriarUseCase(PerfilUsuario.Proprietario);

        await Assert.ThrowsAsync<ForbiddenException>(() => useCase.ExecutarAsync(Request(PerfilUsuario.Proprietario)));
    }

    [Fact]
    public async Task Administrador_PodeCadastrarEngenheiro()
    {
        var resultado = await CriarUseCase(PerfilUsuario.Administrador).ExecutarAsync(Request(PerfilUsuario.Engenheiro));

        Assert.Equal("Engenheiro", resultado.Usuario.Perfil);
    }
}
