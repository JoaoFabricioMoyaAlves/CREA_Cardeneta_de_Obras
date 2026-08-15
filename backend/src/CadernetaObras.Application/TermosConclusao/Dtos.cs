using CadernetaObras.Application.Obras;

namespace CadernetaObras.Application.TermosConclusao;

public record CriarTermoRequest(int ObraId, DateOnly DataConclusao, string Declaracao);

public record TermoResponse(
    int Id,
    int ObraId,
    DateOnly DataConclusao,
    string Declaracao,
    string Status,
    List<AssinaturaResumoDto> Assinaturas
);
