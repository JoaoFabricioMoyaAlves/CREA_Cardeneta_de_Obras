namespace CadernetaObras.Domain.Exceptions;

// Lançada quando uma regra de negócio central é violada (ex: tentar
// alterar uma entidade já assinada). Diferente de erro de validação de
// entrada — representa uma tentativa de burlar uma invariante do domínio.
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class EntidadeImutavelException : DomainException
{
    public EntidadeImutavelException(string entidade)
        : base($"{entidade} já possui assinatura registrada e não pode mais ser alterada ou excluída.") { }
}

public class AssinaturaDuplicadaException : DomainException
{
    public AssinaturaDuplicadaException()
        : base("Este usuário já assinou este documento.") { }
}
