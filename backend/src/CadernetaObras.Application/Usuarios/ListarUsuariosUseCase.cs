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
        if (_currentUser.Perfil != PerfilUsuario.Administrador)
            throw new ForbiddenException("Somente o Administrador do CREA pode listar usuários.");

        var usuarios = await _usuarios.ListarAsync(ct);

        return usuarios
            .Select(u => new UsuarioResponse(
                u.Id, u.Nome, u.Cpf, u.Email, u.Telefone, u.Perfil.ToString(), u.TituloProfissional, u.NumeroRegistro))
            .ToList();
    }
}
