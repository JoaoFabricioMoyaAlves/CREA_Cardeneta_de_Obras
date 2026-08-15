namespace CadernetaObras.Application.Obras;

public record CriarObraRequest(
    Guid ProprietarioId,
    Guid ProfissionalId,
    string LocalObra,
    string Cidade,
    string NumeroRt,
    decimal AreaConstruirM2,
    decimal AreaAmpliarM2,
    decimal AreaReformarM2,
    decimal AreaRegularizarM2,
    string TipoEdificacao,
    string? TipoEdificacaoOutros,
    bool AtivTecnicaDirecao,
    bool AtivTecnicaExecucao,
    bool AtivTecnicaFiscalizacao,
    bool AtivTecnicaProjeto,
    decimal ValorObra,
    DateOnly DataReciboAbertura,
    string? NomeEmpresa,
    string? CnpjEmpresa
);

public record AssinaturaResumoDto(string Papel, Guid UsuarioId, string NomeUsuario, DateTime? Data, string? Hash);

public record ObraResponse(
    int Id,
    string NumeroCaderneta,
    string LocalObra,
    string Cidade,
    string NumeroRt,
    string TipoEdificacao,
    decimal AreaTotalEdificadaM2,
    decimal ValorObra,
    string Status,
    Guid ProfissionalId,
    string NomeProfissional,
    Guid ProprietarioId,
    string NomeProprietario,
    string? NomeEmpresa,
    string? CnpjEmpresa,
    DateOnly DataReciboAbertura,
    List<string> AtividadesTecnicas,
    List<AssinaturaResumoDto> Assinaturas
);
