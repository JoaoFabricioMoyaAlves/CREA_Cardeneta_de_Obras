namespace CadernetaObras.Application.Interfaces;

public record EmailMensagem(string ParaEmail, string ParaNome, string Assunto, string CorpoHtml, string CorpoTexto);

// Envio best-effort, igual à TSA: a mensagem entra numa fila e é enviada em
// segundo plano. Falha de SMTP nunca derruba a operação que gerou o e-mail
// (criar obra, assinar, etc.) — só fica registrada no log.
public interface IEmailService
{
    // false quando não há SMTP configurado (ex.: ambiente local sem Mailpit).
    bool Configurado { get; }

    void Enfileirar(EmailMensagem mensagem);
}
