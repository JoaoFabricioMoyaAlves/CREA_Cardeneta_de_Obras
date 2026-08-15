using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.Obras;

public class ListarObrasUseCase
{
    private readonly IObraRepository _obras;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICurrentUserService _currentUser;

    public ListarObrasUseCase(IObraRepository obras, IUsuarioRepository usuarios, ICurrentUserService currentUser)
    {
        _obras = obras;
        _usuarios = usuarios;
        _currentUser = currentUser;
    }

    public async Task<List<ObraResponse>> ExecutarAsync(CancellationToken ct = default)
    {
        var obras = await _obras.ListarVisiveisAsync(_currentUser.UsuarioId, _currentUser.Perfil, ct);
        var respostas = new List<ObraResponse>();

        foreach (var obra in obras)
        {
            var profissional = await _usuarios.ObterPorIdAsync(obra.IdProfissional, ct);
            var proprietario = await _usuarios.ObterPorIdAsync(obra.IdProprietario, ct);
            if (profissional is null || proprietario is null) continue;
            respostas.Add(ObraMapper.ToResponse(obra, profissional, proprietario));
        }

        return respostas;
    }
}
