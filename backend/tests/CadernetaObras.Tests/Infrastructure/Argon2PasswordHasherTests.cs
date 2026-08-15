using CadernetaObras.Infrastructure.Auth;
using Xunit;

namespace CadernetaObras.Tests.Infrastructure;

public class Argon2PasswordHasherTests
{
    private readonly Argon2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_GeraHashDiferenteDaSenhaOriginal()
    {
        var hash = _hasher.Hash("MinhaSenha123");
        Assert.NotEqual("MinhaSenha123", hash);
        Assert.Contains('.', hash); // formato salt.hash
    }

    [Fact]
    public void Verificar_RetornaTrue_QuandoSenhaCorreta()
    {
        var hash = _hasher.Hash("MinhaSenha123");
        Assert.True(_hasher.Verificar("MinhaSenha123", hash));
    }

    [Fact]
    public void Verificar_RetornaFalse_QuandoSenhaIncorreta()
    {
        var hash = _hasher.Hash("MinhaSenha123");
        Assert.False(_hasher.Verificar("SenhaErrada", hash));
    }

    [Fact]
    public void Hash_GeraSaltDiferenteACadaChamada_MesmaSenhaProduzHashesDiferentes()
    {
        var hash1 = _hasher.Hash("MesmaSenha");
        var hash2 = _hasher.Hash("MesmaSenha");
        Assert.NotEqual(hash1, hash2);
    }
}
