using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Registros;

public class ObterUrlImagemUseCase
{
    private readonly IRelatoVisitaRepository _registros;
    private readonly IObraRepository _obras;
    private readonly IStorageService _storage;
    private readonly ICurrentUserService _currentUser;

    public ObterUrlImagemUseCase(
        IRelatoVisitaRepository registros, IObraRepository obras, IStorageService storage, ICurrentUserService currentUser)
    {
        _registros = registros;
        _obras = obras;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<string> ExecutarAsync(int registroId, int imagemId, CancellationToken ct = default)
    {
        var registro = await _registros.ObterPorIdAsync(registroId, ct)
            ?? throw new NotFoundException("Registro de visita não encontrado.");

        var imagem = registro.Imagens.FirstOrDefault(i => i.Id == imagemId)
            ?? throw new NotFoundException("Imagem não encontrada.");

        var obra = await _obras.ObterPorIdAsync(registro.ObraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && obra.IdProprietario == _currentUser.UsuarioId);
        if (!podeVer)
            throw new ForbiddenException("Esta imagem não está disponível para o seu usuário.");

        return await _storage.ObterUrlTemporariaAsync(imagem.StorageKey, TimeSpan.FromMinutes(15), ct);
    }
}
