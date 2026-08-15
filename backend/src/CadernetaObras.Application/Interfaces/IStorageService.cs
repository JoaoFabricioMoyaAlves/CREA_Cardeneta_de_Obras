namespace CadernetaObras.Application.Interfaces;

public interface IStorageService
{
    Task<string> SalvarArquivoAsync(string nomeArquivo, Stream conteudo, string contentType, CancellationToken ct = default);
    Task<Stream> ObterArquivoAsync(string storageKey, CancellationToken ct = default);
    Task<string> ObterUrlTemporariaAsync(string storageKey, TimeSpan validade, CancellationToken ct = default);
}
