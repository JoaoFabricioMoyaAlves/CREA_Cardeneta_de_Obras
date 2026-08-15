using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Registros;

public class ListarRegistrosUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ListarRegistrosUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _registros = registros;
        _obras = obras;
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<List<RegistroResponse>> ExecutarAsync(int obraId, CancellationToken ct = default)
    {
        var obra = await _obras.ObterPorIdAsync(obraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && obra.IdProprietario == _currentUser.UsuarioId);
        if (!podeVer)
            throw new ForbiddenException("Esta caderneta não está atribuída ao seu usuário.");

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);

        var registros = await _registros.ListarPorObraAsync(obraId, ct);
        return registros.Select(r => RegistroMapper.ToResponse(r, profissional!, proprietario!)).ToList();
    }
}
