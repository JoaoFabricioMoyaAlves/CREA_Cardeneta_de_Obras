using CadernetaObras.Application.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace CadernetaObras.Infrastructure.Storage;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _client;
    private readonly MinioOptions _options;

    public MinioStorageService(IOptions<MinioOptions> options)
    {
        _options = options.Value;
        _client = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSsl)
            .Build();
    }

    private async Task GarantirBucketAsync(CancellationToken ct)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_options.BucketName);
        if (!await _client.BucketExistsAsync(existsArgs, ct))
        {
            var makeArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
            await _client.MakeBucketAsync(makeArgs, ct);
        }
    }

    public async Task<string> SalvarArquivoAsync(string nomeArquivo, Stream conteudo, string contentType, CancellationToken ct = default)
    {
        await GarantirBucketAsync(ct);

        var putArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(nomeArquivo)
            .WithStreamData(conteudo)
            .WithObjectSize(conteudo.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, ct);
        return nomeArquivo;
    }

    public async Task<Stream> ObterArquivoAsync(string storageKey, CancellationToken ct = default)
    {
        var memoryStream = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _client.GetObjectAsync(getArgs, ct);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<string> ObterUrlTemporariaAsync(string storageKey, TimeSpan validade, CancellationToken ct = default)
    {
        var presignedArgs = new PresignedGetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey)
            .WithExpiry((int)validade.TotalSeconds);

        return await _client.PresignedGetObjectAsync(presignedArgs);
    }
}
