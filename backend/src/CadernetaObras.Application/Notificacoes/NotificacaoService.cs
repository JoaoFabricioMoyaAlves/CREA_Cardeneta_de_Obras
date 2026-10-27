using System.Net;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace CadernetaObras.Application.Notificacoes;

public class NotificacaoService : INotificacaoService
{
    private readonly IEmailService _email;
    private readonly string _frontendUrl;

    public NotificacaoService(IEmailService email, IConfiguration configuration)
    {
        _email = email;
        _frontendUrl = (configuration["App:FrontendUrl"] ?? "http://localhost:8080").TrimEnd('/');
    }

    public bool EmailAtivo => _email.Configurado;

    public void PrimeiroAcesso(Usuario usuario, string token)
    {
        var link = $"{_frontendUrl}/redefinir-senha?token={Uri.EscapeDataString(token)}&primeiroAcesso=true";
        Enviar(usuario, "Seu acesso à Caderneta de Obras Digital",
            $"Olá, {usuario.Nome}!",
            [
                "Uma conta foi criada para você na Caderneta de Obras Digital.",
                $"Seu login é o seu CPF: {usuario.Cpf}.",
                "Clique no botão abaixo para criar a sua senha. O link vale por 72 horas e só pode ser usado uma vez.",
            ],
            "Criar minha senha", link);
    }

    public void RecuperacaoSenha(Usuario usuario, string token)
    {
        var link = $"{_frontendUrl}/redefinir-senha?token={Uri.EscapeDataString(token)}";
        Enviar(usuario, "Redefinição de senha — Caderneta de Obras Digital",
            $"Olá, {usuario.Nome}!",
            [
                "Recebemos um pedido para redefinir a senha da sua conta.",
                "Clique no botão abaixo para escolher uma nova senha. O link vale por 1 hora e só pode ser usado uma vez.",
                "Se você não fez esse pedido, ignore este e-mail — sua senha atual continua valendo.",
            ],
            "Redefinir minha senha", link);
    }

    public void DocumentoCriado(Usuario proprietario, Usuario engenheiro, Obra obra, DocumentoParaAssinar documento, string caminho)
    {
        var (nomeDocumento, artigo) = Descrever(documento);
        Enviar(proprietario, $"{Capitalizar(nomeDocumento)} aguardando sua assinatura — {obra.NumeroCaderneta}",
            $"Olá, {proprietario.Nome}!",
            [
                $"{engenheiro.Nome} registrou {artigo} {nomeDocumento} na caderneta {obra.NumeroCaderneta} ({obra.LocalObra}, {obra.Cidade}).",
                "Sua assinatura como proprietário é necessária. Acesse o sistema para conferir e assinar.",
            ],
            "Ver e assinar", $"{_frontendUrl}{caminho}");
    }

    public void EngenheiroAssinou(Usuario proprietario, Usuario engenheiro, Obra obra, DocumentoParaAssinar documento, string caminho)
    {
        var (nomeDocumento, artigo) = Descrever(documento);
        Enviar(proprietario, $"Falta só a sua assinatura — {obra.NumeroCaderneta}",
            $"Olá, {proprietario.Nome}!",
            [
                $"{engenheiro.Nome} já assinou {artigo} {nomeDocumento} da caderneta {obra.NumeroCaderneta} ({obra.LocalObra}, {obra.Cidade}).",
                "Agora falta apenas a sua assinatura para concluir esta etapa.",
            ],
            "Assinar agora", $"{_frontendUrl}{caminho}");
    }

    private static (string Nome, string Artigo) Descrever(DocumentoParaAssinar documento) => documento switch
    {
        DocumentoParaAssinar.AberturaObra => ("termo de abertura", "o"),
        DocumentoParaAssinar.RelatoVisita => ("relato de visita", "o"),
        DocumentoParaAssinar.TermoConclusao => ("termo de conclusão", "o"),
        _ => ("documento", "o"),
    };

    private static string Capitalizar(string texto) => char.ToUpper(texto[0]) + texto[1..];

    private void Enviar(Usuario destinatario, string assunto, string saudacao, string[] paragrafos, string textoBotao, string link)
    {
        var texto = $"{saudacao}\n\n{string.Join("\n\n", paragrafos)}\n\n{textoBotao}: {link}\n\n— Caderneta de Obras Digital";

        var html = $"""
            <div style="font-family:Arial,Helvetica,sans-serif;max-width:560px;margin:0 auto;color:#1f2937">
              <h2 style="color:#1e3a5f;margin-bottom:16px">Caderneta de Obras Digital</h2>
              <p style="font-size:15px">{WebUtility.HtmlEncode(saudacao)}</p>
              {string.Concat(paragrafos.Select(p => $"<p style=\"font-size:15px;line-height:1.5\">{WebUtility.HtmlEncode(p)}</p>"))}
              <p style="margin:28px 0">
                <a href="{WebUtility.HtmlEncode(link)}" style="background:#1e3a5f;color:#ffffff;padding:12px 22px;border-radius:6px;text-decoration:none;font-weight:bold;display:inline-block">{WebUtility.HtmlEncode(textoBotao)}</a>
              </p>
              <p style="font-size:12px;color:#6b7280">Se o botão não funcionar, copie e cole este endereço no navegador:<br>{WebUtility.HtmlEncode(link)}</p>
              <p style="font-size:12px;color:#6b7280">Este é um e-mail automático, não responda.</p>
            </div>
            """;

        _email.Enfileirar(new EmailMensagem(destinatario.Email, destinatario.Nome, assunto, html, texto));
    }
}
