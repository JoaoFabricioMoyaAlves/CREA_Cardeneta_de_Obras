using System.Threading.Channels;
using CadernetaObras.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace CadernetaObras.Infrastructure.Email;

// Fila em memória: a requisição HTTP só enfileira e segue; o EmailDispatcher
// envia em segundo plano. Mensagens pendentes se perdem se a API reiniciar —
// aceitável aqui, pois o usuário sempre pode pedir um novo link e o sistema
// continua mostrando o que falta assinar.
public class FilaEmailService : IEmailService
{
    private readonly Channel<EmailMensagem> _fila = Channel.CreateBounded<EmailMensagem>(
        new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.DropOldest });
    private readonly EmailOptions _options;

    public FilaEmailService(IOptions<EmailOptions> options) => _options = options.Value;

    public bool Configurado => !string.IsNullOrWhiteSpace(_options.Host);

    public ChannelReader<EmailMensagem> Leitor => _fila.Reader;

    public void Enfileirar(EmailMensagem mensagem) => _fila.Writer.TryWrite(mensagem);
}
