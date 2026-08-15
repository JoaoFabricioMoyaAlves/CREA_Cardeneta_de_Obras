using CadernetaObras.Infrastructure.Services;
using Xunit;

namespace CadernetaObras.Tests.Infrastructure;

public class Sha256HashServiceTests
{
    private readonly Sha256HashService _service = new();

    [Fact]
    public void GerarHashSha256_MesmoConteudo_GeraMesmoHash()
    {
        var hash1 = _service.GerarHashSha256("obra-1|local|100.00");
        var hash2 = _service.GerarHashSha256("obra-1|local|100.00");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GerarHashSha256_ConteudoDiferente_GeraHashDiferente()
    {
        // Prova o núcleo da assinatura: qualquer alteração no conteúdo
        // (ex: adulteração após assinatura) quebra o hash.
        var hashOriginal = _service.GerarHashSha256("obra-1|local|100.00");
        var hashAdulterado = _service.GerarHashSha256("obra-1|local|999.00");
        Assert.NotEqual(hashOriginal, hashAdulterado);
    }

    [Fact]
    public void GerarHashSha256_BateComVetorDeTesteConhecido()
    {
        // SHA-256("abc") é um vetor de teste padrão (NIST).
        var hash = _service.GerarHashSha256("abc");
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }
}
