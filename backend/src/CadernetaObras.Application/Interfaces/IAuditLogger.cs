namespace CadernetaObras.Application.Interfaces;

// Registra uma entrada de auditoria (fica pendente no change tracker — o
// SaveChanges do próprio use case que chamou persiste tudo junto, na mesma
// transação da ação que está sendo auditada).
public interface IAuditLogger
{
    void Registrar(string acao, Guid? usuarioId, string? entidadeTipo = null, string? entidadeId = null, string? detalhes = null);
}
