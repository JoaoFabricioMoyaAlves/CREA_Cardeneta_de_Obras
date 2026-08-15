using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Obras;

public class ObterObraUseCase
{
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ObterObraUseCase(IObraRepository obras, IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _obras = obras;
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<ObraResponse> ExecutarAsync(int obraId, CancellationToken ct = default)
    {
        var obra = await _obras.ObterComAssinaturasAsync(obraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && obra.IdProprietario == _currentUser.UsuarioId);

        if (!podeVer)
            throw new ForbiddenException("Esta caderneta não está atribuída ao seu usuário.");

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct)
            ?? throw new NotFoundException("Responsável técnico da obra não encontrado.");
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct)
            ?? throw new NotFoundException("Proprietário da obra não encontrado.");

        return ObraMapper.ToResponse(obra, profissional, proprietario);
    }
}
