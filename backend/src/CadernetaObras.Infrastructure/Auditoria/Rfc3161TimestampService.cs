using System.Net.Http.Headers;
using CadernetaObras.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;

namespace CadernetaObras.Infrastructure.Auditoria;

// Cliente RFC 3161 via BouncyCastle: monta o TimeStampRequest (ASN.1/DER)
// com o hash já calculado, envia pra TSA configurada, valida a resposta
// contra o próprio request (garante que a TSA carimbou o hash certo) e
// devolve o token assinado (guardado como evidência extra na assinatura).
public class Rfc3161TimestampService : ITimestampAuthorityService
{
    private static readonly MediaTypeHeaderValue ContentType = new("application/timestamp-query");

    private readonly HttpClient _httpClient;
    private readonly TsaOptions _options;
    private readonly ILogger<Rfc3161TimestampService> _logger;

    public Rfc3161TimestampService(HttpClient httpClient, IOptions<TsaOptions> options, ILogger<Rfc3161TimestampService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CarimboTempo?> ObterCarimboAsync(byte[] hashSha256, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Url)) return null;

        try
        {
            var geradorRequisicao = new TimeStampRequestGenerator();
            geradorRequisicao.SetCertReq(true);
            var nonce = new BigInteger(64, new Random());
            var requisicao = geradorRequisicao.Generate(TspAlgorithms.Sha256, hashSha256, nonce);

            using var conteudo = new ByteArrayContent(requisicao.GetEncoded());
            conteudo.Headers.ContentType = ContentType;

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSegundos));

            using var respostaHttp = await _httpClient.PostAsync(_options.Url, conteudo, timeoutCts.Token);
            if (!respostaHttp.IsSuccessStatusCode)
            {
                _logger.LogWarning("TSA {Url} respondeu {Status} — assinatura segue sem carimbo externo.", _options.Url, respostaHttp.StatusCode);
                return null;
            }

            var respostaBytes = await respostaHttp.Content.ReadAsByteArrayAsync(ct);
            var resposta = new TimeStampResponse(respostaBytes);
            resposta.Validate(requisicao);

            var token = resposta.TimeStampToken;
            if (token is null) return null;

            var genTime = token.TimeStampInfo.GenTime;
            var tokenBase64 = Convert.ToBase64String(token.GetEncoded());
            var autoridade = new Uri(_options.Url).Host;

            return new CarimboTempo(tokenBase64, genTime.ToUniversalTime(), autoridade);
        }
        catch (Exception ex)
        {
            // Best-effort: qualquer falha (rede, timeout, TSA fora do ar,
            // resposta inválida) não pode bloquear o fluxo de assinatura —
            // o hash + timestamp do servidor já são válidos por si só.
            _logger.LogWarning(ex, "Falha ao obter carimbo de tempo RFC 3161 em {Url}.", _options.Url);
            return null;
        }
    }
}
