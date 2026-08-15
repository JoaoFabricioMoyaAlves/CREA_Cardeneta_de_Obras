namespace CadernetaObras.Application.Common;

// Exceções da camada Application, mapeadas para status HTTP no middleware
// de tratamento de erros da API (Domain fica com DomainException, que é
// especificamente sobre violação de invariantes como imutabilidade).
public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string mensagem) : base(mensagem) { }
}

public class ValidationAppException : Exception
{
    public ValidationAppException(string mensagem) : base(mensagem) { }
}

public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string mensagem) : base(mensagem) { }
}
