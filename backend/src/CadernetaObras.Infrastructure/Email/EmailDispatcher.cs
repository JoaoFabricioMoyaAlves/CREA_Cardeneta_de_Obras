using CadernetaObras.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CadernetaObras.Infrastructure.Email;

public class EmailDispatcher : BackgroundService
{
    private const int Tentativas = 3;

    private readonly FilaEmailService _fila;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailDispatcher> _logger;

    public EmailDispatcher(FilaEmailService fila, IOptions<EmailOptions> options, ILogger<EmailDispatcher> logger)
    {
        _fila = fila;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var mensagem in _fila.Leitor.ReadAllAsync(stoppingToken))
        {
            if (!_fila.Configurado)
            {
                // Sem SMTP: registra no log para dar para testar os links localmente.
                _logger.LogInformation("E-mail (SMTP não configurado) para {Para} — {Assunto}\n{Corpo}",
                    mensagem.ParaEmail, mensagem.Assunto, mensagem.CorpoTexto);
                continue;
            }

            for (var tentativa = 1; tentativa <= Tentativas; tentativa++)
            {
                try
                {
                    await EnviarAsync(mensagem, stoppingToken);
                    _logger.LogInformation("E-mail enviado para {Para} — {Assunto}", mensagem.ParaEmail, mensagem.Assunto);
                    break;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Falha ao enviar e-mail para {Para} (tentativa {Tentativa}/{Total}).",
                        mensagem.ParaEmail, tentativa, Tentativas);
                    if (tentativa < Tentativas)
                        await Task.Delay(TimeSpan.FromSeconds(5 * tentativa), stoppingToken);
                }
            }
        }
    }

    private async Task EnviarAsync(EmailMensagem mensagem, CancellationToken ct)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_options.RemetenteNome, string.IsNullOrWhiteSpace(_options.RemetenteEmail) ? _options.Usuario : _options.RemetenteEmail));
        mime.To.Add(new MailboxAddress(mensagem.ParaNome, mensagem.ParaEmail));
        mime.Subject = mensagem.Assunto;
        mime.Body = new BodyBuilder { HtmlBody = mensagem.CorpoHtml, TextBody = mensagem.CorpoTexto }.ToMessageBody();

        var seguranca = _options.Seguranca switch
        {
            "SslOnConnect" => SecureSocketOptions.SslOnConnect,
            "Nenhuma" => SecureSocketOptions.None,
            _ => SecureSocketOptions.StartTls,
        };

        using var smtp = new SmtpClient { Timeout = 20_000 };
        await smtp.ConnectAsync(_options.Host, _options.Porta, seguranca, ct);
        if (!string.IsNullOrWhiteSpace(_options.Usuario))
            await smtp.AuthenticateAsync(_options.Usuario, _options.Senha, ct);
        await smtp.SendAsync(mime, ct);
        await smtp.DisconnectAsync(true, ct);
    }
}
