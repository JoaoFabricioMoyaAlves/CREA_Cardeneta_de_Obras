using System.Text.Json;

namespace CadernetaObras.Application.Documentos;

public record AssinarDocumentoRequest(string Cpf, string Senha, string HashDocumentoVisualizado);

public record AssinaturaDocumentoDto(
    string Papel,
    string Nome,
    DateTime Data,
    string Hash,
    string Ip,
    bool CarimboVerificado,
    string? CarimboAutoridade);

public record DocumentoResponse(
    string Tipo,
    int ReferenciaId,
    int ModeloVersao,
    bool Congelado,
    bool AnteriorAoModelo,
    string HashDocumento,
    JsonElement Dados,
    List<AssinaturaDocumentoDto> Assinaturas,
    string? PapelQuePodeAssinar);
