namespace CadernetaObras.Domain.Enums;

// Papel exercido no momento da assinatura, derivado do PerfilUsuario.
// Existe como enum separado porque uma entidade pode, no futuro, exigir
// assinatura de um papel que não é 1:1 com o perfil de login (ex.: um
// Administrador testemunhando um evento). Hoje só Engenheiro/Proprietario
// assinam.
public enum PapelAssinatura
{
    Engenheiro = 1,
    Proprietario = 2,
}
