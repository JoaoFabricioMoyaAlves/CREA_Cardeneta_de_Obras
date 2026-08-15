using System.Security.Cryptography;
using System.Text;
using CadernetaObras.Application.Interfaces;

namespace CadernetaObras.Infrastructure.Services;

public class Sha256HashService : IHashService
{
    public string GerarHashSha256(string conteudoCanonico)
    {
        var bytes = Encoding.UTF8.GetBytes(conteudoCanonico);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hash);
    }
}
