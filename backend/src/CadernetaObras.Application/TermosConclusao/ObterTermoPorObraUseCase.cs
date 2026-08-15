using CadernetaObras.Application.Common;
using CadernetaObras.Application.Interfaces;
using CadernetaObras.Application.Obras;
using CadernetaObras.Domain.Enums;
using CadernetaObras.Domain.Interfaces;

namespace CadernetaObras.Application.TermosConclusao;

public class ObterTermoPorObraUseCase
{
    private readonly ITermoConclusaoRepository _termos;
    private readonly IObraRepository _obras;
    private readonly ICurrentUserService _currentUser;

    public ObterTermoPorObraUseCase(ITermoConclusaoRepository termos, IObraRepository obras, ICurrentUserService currentUser)
    {
        _termos = termos;
        _obras = obras;
        _currentUser = currentUser;
    }

    // Retorna null quando a obra ainda não tem termo emitido — não é erro,
    // é um estado normal (a maioria das obras ativas não tem termo ainda).
    public async Task<TermoResponse?> ExecutarAsync(int obraId, CancellationToken ct = default)
    {
        var obra = await _obras.ObterPorIdAsync(obraId, ct)
            ?? throw new NotFoundException("Caderneta não encontrada.");

        var podeVer = _currentUser.Perfil == PerfilUsuario.Administrador
            || (_currentUser.Perfil == PerfilUsuario.Engenheiro && obra.IdProfissional == _currentUser.UsuarioId)
            || (_currentUser.Perfil == PerfilUsuario.Proprietario && obra.IdProprietario == _currentUser.UsuarioId);
        if (!podeVer)
            throw new ForbiddenException("Esta caderneta não está atribuída ao seu usuário.");

        var termoPorObra = await _termos.ObterPorObraIdAsync(obraId, ct);
        if (termoPorObra is null) return null;

        var completo = await _termos.ObterComAssinaturasAsync(termoPorObra.Id, ct);
        if (completo is null) return null;

        var assinaturas = completo.Assinaturas
            .Select(a => new AssinaturaResumoDto(
                a.Papel.ToString(),
                a.UsuarioId,
                "", // nomes resolvidos no frontend a partir da própria Obra (profissional/proprietário)
                a.Data,
                a.CodHash))
            .ToList();

        return new TermoResponse(completo.Id, completo.ObraId, completo.DataConclusao, completo.Declaracao, completo.Status.ToString(), assinaturas);
    }
}
