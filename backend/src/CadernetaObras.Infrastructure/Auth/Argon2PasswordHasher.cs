using System.Security.Cryptography;
using CadernetaObras.Application.Interfaces;
using Konscious.Security.Cryptography;

namespace CadernetaObras.Infrastructure.Auth;

// Argon2id com os parâmetros-base recomendados pelo OWASP Password Storage
// Cheat Sheet (m=19456 KiB, t=2, p=1) para novos projetos em 2026.
public class Argon2PasswordHasher : IPasswordHasher
{
    private const int MemorySizeKb = 19456;
    private const int Iterations = 2;
    private const int Parallelism = 1;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = DerivarHash(senha, salt);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string hashArmazenado)
    {
        var partes = hashArmazenado.Split('.');
        if (partes.Length != 2) return false;

        var salt = Convert.FromBase64String(partes[0]);
        var hashEsperado = Convert.FromBase64String(partes[1]);
        var hashCalculado = DerivarHash(senha, salt);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }

    private static byte[] DerivarHash(string senha, byte[] salt)
    {
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(senha))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            Iterations = Iterations,
            MemorySize = MemorySizeKb,
        };
        return argon2.GetBytes(HashSize);
    }
}
