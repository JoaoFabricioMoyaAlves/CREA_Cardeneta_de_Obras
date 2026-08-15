using CadernetaObras.Domain.Entities;
using CadernetaObras.Application.Obras;

namespace CadernetaObras.Application.Registros;

internal static class RegistroMapper
{
    public static RegistroResponse ToResponse(RelatoVisita registro, Usuario profissional, Usuario proprietario)
    {
        var fases = new List<string>();
        if (registro.FaseServicosPreliminares) fases.Add("Serviços preliminares");
        if (registro.FaseFundacao) fases.Add("Fundação");
        if (registro.FaseAlvenarias) fases.Add("Alvenaria");
        if (registro.FaseSuperestrutura) fases.Add("Superestrutura");
        if (registro.FaseCobertura) fases.Add("Cobertura");
        if (registro.FaseEsquadriasInst) fases.Add("Esquadrias e instalações");
        if (registro.FaseRevestimento) fases.Add("Revestimento");
        if (registro.FasePintura) fases.Add("Pintura");
        if (registro.FaseServicosComp) fases.Add("Serviços complementares");

        var assinaturas = registro.Assinaturas
            .Select(a => new AssinaturaResumoDto(
                a.Papel.ToString(),
                a.UsuarioId,
                a.UsuarioId == profissional.Id ? profissional.Nome : proprietario.Nome,
                a.Data,
                a.CodHash))
            .ToList();

        var imagens = registro.Imagens
            .Select(i => new ImagemResponse(i.Id, i.Name, i.StorageKey, i.Data))
            .ToList();

        return new RegistroResponse(
            registro.Id, registro.ObraId, registro.DataVisita, registro.PosicaoObra,
            registro.DecisoesOrientacoes, fases, registro.Status.ToString(), imagens, assinaturas);
    }
}
