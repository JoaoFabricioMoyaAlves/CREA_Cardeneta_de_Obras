using System.Text.Encodings.Web;
using System.Text.Json;

namespace CadernetaObras.Application.Documentos;

public record ProfissionalDocumento(string Nome, string? Titulo, string? Registro);
public record EmpresaDocumento(string? Nome, string? Cnpj);
public record ProprietarioDocumento(string Nome, string Cpf, string Telefone);

public record DadosAbertura(
    string NumeroCaderneta,
    ProfissionalDocumento Profissional,
    EmpresaDocumento Empresa,
    ProprietarioDocumento Proprietario,
    string LocalObra,
    string Cidade,
    string Estado,
    decimal AreaConstruirM2,
    decimal AreaAmpliarM2,
    decimal AreaReformarM2,
    decimal AreaRegularizarM2,
    decimal AreaTotalEdificadaM2,
    string TipoEdificacao,
    string? TipoEdificacaoOutros,
    IReadOnlyList<string> AtividadesTecnicas,
    string NumeroRt,
    decimal ValorObra,
    DateOnly DataRecibo);

public record FotoDocumento(int Id, string Nome, string? Sha256);

public record DadosRelato(
    string NumeroCaderneta,
    int Sequencia,
    string LocalObra,
    string Cidade,
    string Estado,
    ProfissionalDocumento Profissional,
    ProprietarioDocumento Proprietario,
    DateOnly DataVisita,
    IReadOnlyList<string> Fases,
    string PosicaoObra,
    string DecisoesOrientacoes,
    IReadOnlyList<FotoDocumento> Fotos);

public record DadosTermo(
    string NumeroCaderneta,
    ProfissionalDocumento Profissional,
    EmpresaDocumento Empresa,
    ProprietarioDocumento Proprietario,
    string LocalObra,
    string Cidade,
    string Estado,
    DateOnly DataConclusao,
    string Declaracao);

public static class DocumentoJson
{
    // Versão do modelo HTML/CSS usada pelos documentos novos. Modelo publicado
    // não muda: alteração visual vira uma nova versão no frontend.
    public const int ModeloVersaoAtual = 1;

    private static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static string Serializar<T>(T dados) => JsonSerializer.Serialize(dados, Opcoes);
}
