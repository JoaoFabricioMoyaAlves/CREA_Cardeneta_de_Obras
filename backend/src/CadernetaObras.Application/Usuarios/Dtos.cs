using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Application.Usuarios;

public record CriarUsuarioRequest(
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    PerfilUsuario Perfil,
    string? TituloProfissional,
    string? NumeroRegistro,
    // Obrigatório apenas quando Perfil == Administrador — proteção extra
    // contra criação indevida de contas com acesso total ao sistema.
    string? SenhaSecretaDev
);

public record UsuarioResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    string Perfil,
    string? TituloProfissional,
    string? NumeroRegistro
);

// SenhaProvisoria só vem preenchida quando não há SMTP configurado (ex.:
// ambiente local). Com e-mail ativo é null: o usuário recebe um link de
// primeiro acesso e cria a própria senha.
public record UsuarioCriadoResponse(UsuarioResponse Usuario, string? SenhaProvisoria);
