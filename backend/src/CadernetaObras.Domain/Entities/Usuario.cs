using CadernetaObras.Domain.Enums;

namespace CadernetaObras.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }

    // Só preenchido quando Perfil == Engenheiro (inclui Arquitetos — o tipo
    // granular "Engenheiro/Arquiteto/especialidade" escolhido no cadastro do
    // frontend é armazenado aqui como texto livre, ex: "Arquiteto e Urbanista").
    public string? TituloProfissional { get; set; }
    public string? NumeroRegistro { get; set; }

    public DateTime CriadoEm { get; set; }
}
