namespace CadernetaObras.Infrastructure.Auditoria;

public class TsaOptions
{
    public const string SectionName = "Tsa";

    // URL de uma autoridade de carimbo de tempo RFC 3161 pública. Vazio
    // desativa o recurso (as assinaturas seguem funcionando normalmente,
    // só sem o carimbo externo). Padrão sugerido: http://timestamp.digicert.com
    public string Url { get; set; } = string.Empty;
    public int TimeoutSegundos { get; set; } = 8;
}
