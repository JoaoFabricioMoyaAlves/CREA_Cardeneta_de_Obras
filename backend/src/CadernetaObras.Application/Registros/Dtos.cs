namespace CadernetaObras.Application.Registros;

public record CriarRegistroRequest(
    int ObraId,
    DateOnly DataVisita,
    string PosicaoObra,
    string DecisoesOrientacoes,
    bool FaseServicosPreliminares,
    bool FaseFundacao,
    bool FaseAlvenarias,
    bool FaseSuperestrutura,
    bool FaseCobertura,
    bool FaseEsquadriasInst,
    bool FaseRevestimento,
    bool FasePintura,
    bool FaseServicosComp
);

public record ImagemResponse(int Id, string Name, string StorageKey, DateTime Data);

public record RegistroResponse(
    int Id,
    int ObraId,
    DateOnly DataVisita,
    string PosicaoObra,
    string DecisoesOrientacoes,
    List<string> Fases,
    string Status,
    List<ImagemResponse> Imagens,
    List<CadernetaObras.Application.Obras.AssinaturaResumoDto> Assinaturas
);
