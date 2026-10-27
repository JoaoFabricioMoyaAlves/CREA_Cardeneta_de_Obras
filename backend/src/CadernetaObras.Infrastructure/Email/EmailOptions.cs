namespace CadernetaObras.Infrastructure.Email;

public class EmailOptions
{
    public const string SectionName = "Email";

    // Host vazio = e-mail desligado (as mensagens só aparecem no log).
    public string Host { get; set; } = string.Empty;
    public int Porta { get; set; } = 587;

    // "StartTls" (porta 587 — a única liberada na VPS; a 465 é bloqueada
    // pela Hetzner), "SslOnConnect" (465) ou "Nenhuma" (Mailpit local).
    public string Seguranca { get; set; } = "StartTls";

    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string RemetenteEmail { get; set; } = string.Empty;
    public string RemetenteNome { get; set; } = "Caderneta de Obras Digital";
}
