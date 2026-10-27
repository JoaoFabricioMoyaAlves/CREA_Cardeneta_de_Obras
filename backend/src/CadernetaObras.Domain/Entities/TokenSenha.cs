using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

// Link de uso único enviado por e-mail para definir (primeiro acesso) ou
// redefinir (recuperação) a senha. Só o hash SHA-256 do token fica no banco —
// quem tiver acesso ao banco não consegue usar um link pendente.
public class TokenSenha
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public FinalidadeTokenSenha Finalidade { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime ExpiraEm { get; set; }
    public DateTime? UsadoEm { get; set; }
}
