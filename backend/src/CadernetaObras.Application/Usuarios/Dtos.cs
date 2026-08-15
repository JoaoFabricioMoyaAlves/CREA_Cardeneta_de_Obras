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

// Retornado só na criação — em produção isso deveria ser enviado por e-mail
// ao usuário, nunca devolvido na resposta da API. Mantido aqui só para
// permitir testar o fluxo de ponta a ponta sem um provedor de e-mail configurado.
public record UsuarioCriadoResponse(UsuarioResponse Usuario, string SenhaProvisoria);
