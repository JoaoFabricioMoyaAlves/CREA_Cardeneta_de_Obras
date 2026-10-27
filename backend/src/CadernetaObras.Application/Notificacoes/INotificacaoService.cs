using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Application.Notificacoes;

public enum DocumentoParaAssinar
{
    AberturaObra,
    RelatoVisita,
    TermoConclusao,
}

public interface INotificacaoService
{
    // false quando não há SMTP configurado — os e-mails só aparecem no log.
    bool EmailAtivo { get; }

    void PrimeiroAcesso(Usuario usuario, string token);
    void RecuperacaoSenha(Usuario usuario, string token);

    // Enviado ao proprietário quando o engenheiro cria o documento.
    void DocumentoCriado(Usuario proprietario, Usuario engenheiro, Obra obra, DocumentoParaAssinar documento, string caminho);

    // Enviado ao proprietário quando o engenheiro já assinou e só falta ele.
    void EngenheiroAssinou(Usuario proprietario, Usuario engenheiro, Obra obra, DocumentoParaAssinar documento, string caminho);
}
