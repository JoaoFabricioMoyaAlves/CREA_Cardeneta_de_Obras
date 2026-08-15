using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Registros;

public class ObterRegistroUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ObterRegistroUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _registros = registros;
        _obras = obras;
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<RegistroResponse> ExecutarAsync(int registroId, CancellationToken ct = default)
    {
        var registro = await _registros.ObterComAssinaturasAsync(registroId, ct)
            ?? throw new NotFoundException("Registro de visita não encontrado.");

        var obra = await _obras.ObterPorIdAsync(registro.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && obra.IdProprietario == _currentUser.UsuarioId);
        if (!podeVer)
            throw new ForbiddenException("Este registro não está disponível para o seu usuário.");

        var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
        var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);

        return RegistroMapper.ToResponse(registro, profissional!, proprietario!);
    }
}
