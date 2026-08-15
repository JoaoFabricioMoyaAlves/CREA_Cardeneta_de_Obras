using CadernetaObras.Domain.Entities;

namespace CadernetaObras.Application.Obras;

internal static class ObraMapper
{
    public static ObraResponse ToResponse(Obra obra, Usuario profissional, Usuario proprietario)
    {
        var atividades = new List<string>();
        if (obra.AtivTecnicaDirecao) atividades.Add("Direção");
        if (obra.AtivTecnicaExecucao) atividades.Add("Execução");
        if (obra.AtivTecnicaFiscalizacao) atividades.Add("Fiscalização");
        if (obra.AtivTecnicaProjeto) atividades.Add("Projeto");

        var assinaturas = obra.Assinaturas
            .Select(a => new AssinaturaResumoDto(
                a.Papel.ToString(),
                a.UsuarioId,
                a.UsuarioId == profissional.Id ? profissional.Nome : proprietario.Nome,
                a.Data,
                a.CodHash,
                a.TsaToken is not null,
                a.TsaAutoridade,
                a.TsaDataHora))
            .ToList();

        return new ObraResponse(
            obra.Id,
            obra.NumeroCaderneta,
            obra.LocalObra,
            obra.Cidade,
            obra.NumeroRt,
            obra.TipoEdificacao,
            obra.AreaConstruirM2,
            obra.AreaAmpliarM2,
            obra.AreaReformarM2,
            obra.AreaRegularizarM2,
            obra.AreaTotalEdificadaM2,
            obra.ValorObra,
            obra.Status.ToString(),
            profissional.Id,
            profissional.Nome,
            proprietario.Id,
            proprietario.Nome,
            obra.NomeEmpresa,
            obra.CnpjEmpresa,
            obra.DataReciboAbertura,
            atividades,
            assinaturas
        );
    }
}
