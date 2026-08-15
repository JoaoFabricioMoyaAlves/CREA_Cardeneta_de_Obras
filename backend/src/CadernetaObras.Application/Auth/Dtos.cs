namespace CadernetaObras.Application.Auth;

public record LoginRequest(string Cpf, string Senha);

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
