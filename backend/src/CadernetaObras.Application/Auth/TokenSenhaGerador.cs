using System.Security.Cryptography;
using System.Text;
using CadernetaObras.Domain.Entities;
using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Application.Auth;

public static class TokenSenhaGerador
{
    public static readonly TimeSpan ValidadePrimeiroAcesso = TimeSpan.FromHours(72);
    public static readonly TimeSpan ValidadeRecuperacao = TimeSpan.FromHours(1);

    // 32 bytes aleatórios em base64url: o token em claro vai só no link do
    // e-mail; a entidade guarda apenas o hash.
    public static (TokenSenha Entidade, string Token) Criar(Guid usuarioId, FinalidadeTokenSenha finalidade)
    {
        var token = Base64Url(RandomNumberGenerator.GetBytes(32));
        var agora = DateTime.UtcNow;
        var entidade = new TokenSenha
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            TokenHash = Hash(token),
            Finalidade = finalidade,
            CriadoEm = agora,
            ExpiraEm = agora + (finalidade == FinalidadeTokenSenha.PrimeiroAcesso ? ValidadePrimeiroAcesso : ValidadeRecuperacao),
        };
        return (entidade, token);
    }

    public static string Hash(string token) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
