using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Usuarios;

public class ListarUsuariosUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ListarUsuariosUseCase(IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<List<UsuarioResponse>> ExecutarAsync(CancellationToken ct = default)
    {
        // Administrador lista para gerenciar cadastros; Engenheiro/Arquiteto lista
        // para escolher o proprietário ao criar uma nova obra.
        if (_currentUser.Perfil != PerfilUsuario.Administrador && _currentUser.Perfil != PerfilUsuario.Engenheiro)
            throw new ForbiddenException("Somente o Administrador do CREA ou um Engenheiro/Arquiteto podem listar usuários.");

        var usuarios = await _usuarios.ListarAsync(ct);

        return usuarios
            .Select(u => new UsuarioResponse(
                u.Id, u.Nome, u.Cpf, u.Email, u.Telefone, u.Perfil.ToString(), u.TituloProfissional, u.NumeroRegistro))
            .ToList();
    }
}
