namespace CadernetaObras.Application.Auth;

public record LoginRequest(string Cpf, string Senha);

public record SolicitarRecuperacaoSenhaRequest(string Email);

public record RedefinirSenhaRequest(string Token, string NovaSenha);

public record LoginResponse(string Token, UsuarioLogadoDto Usuario);

public record UsuarioLogadoDto(
    Guid Id,
    string Nome,
    string Cpf,
    string Email,
    string Perfil,
    string? TituloProfissional,
    string? NumeroRegistro
);
