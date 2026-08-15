namespace CadernetaObras.Domain.Entities;

// Registro de auditoria append-only (RF09) — nunca é editado ou excluído,
// nem pelo Administrador (mesmo trigger de imutabilidade das assinaturas).
public class LogAuditoria
{
    public int Id { get; set; }
    public DateTime DataHoraUtc { get; set; }

    // Nulo quando a ação ocorre antes de existir um usuário autenticado
    // (ex: tentativa de login com credenciais inválidas).
    public Guid? UsuarioId { get; set; }

    public string Acao { get; set; } = string.Empty;
    public string? EntidadeTipo { get; set; }
    public string? EntidadeId { get; set; }
    public string? Detalhes { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
