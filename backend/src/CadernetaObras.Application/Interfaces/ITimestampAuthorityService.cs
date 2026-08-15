namespace CadernetaObras.Application.Interfaces;

public record CarimboTempo(string TokenBase64, DateTime DataHoraTsa, string Autoridade);

// Carimbo de tempo RFC 3161: uma autoridade externa (TSA) assina
// criptograficamente "este hash existia neste exato momento", reforçando o
// não-repúdio além do timestamp do nosso próprio servidor. É estritamente
// opcional/best-effort — se a TSA estiver fora do ar, a assinatura segue
// válida só com hash + IP + user-agent + timestamp do servidor (retorna
// null em vez de lançar exceção, para nunca bloquear o fluxo de assinar).
public interface ITimestampAuthorityService
{
    Task<CarimboTempo?> ObterCarimboAsync(byte[] hashSha256, CancellationToken ct = default);
}
