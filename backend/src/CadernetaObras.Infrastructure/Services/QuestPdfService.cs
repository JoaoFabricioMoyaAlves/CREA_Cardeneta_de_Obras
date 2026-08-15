using CadernetaObras.Application.Interfaces;
using CadernetaObras.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CadernetaObras.Infrastructure.Services;

public class QuestPdfService : IPdfService
{
    static QuestPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GerarPdfRegistro(RelatoVisita registro, Obra obra)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text($"Registro de Visita — {obra.NumeroCaderneta}")
                    .SemiBold().FontSize(16);

                page.Content().Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text($"Obra: {obra.LocalObra} — {obra.Cidade}");
                    col.Item().Text($"Data da visita: {registro.DataVisita:dd/MM/yyyy}");
                    col.Item().Text($"Posição da obra: {registro.PosicaoObra}");
                    col.Item().Text($"Decisões e orientações: {registro.DecisoesOrientacoes}");

                    col.Item().PaddingTop(10).Text("Assinaturas").SemiBold();
                    foreach (var a in registro.Assinaturas)
                    {
                        col.Item().Text(
                            $"{a.Papel} — assinado em {a.Data:dd/MM/yyyy HH:mm} UTC — hash {a.CodHash[..Math.Min(16, a.CodHash.Length)]}...");
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Documento gerado eletronicamente — Caderneta de Obras Digital / CREA");
                });
            });
        }).GeneratePdf();
    }

    public byte[] GerarPdfCaderneta(Obra obra, List<RelatoVisita> registros, TermoConclusao? termo)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text($"Caderneta de Obra — {obra.NumeroCaderneta}").SemiBold().FontSize(16);

                page.Content().Column(col =>
                {
                    col.Spacing(6);
                    col.Item().Text($"Local: {obra.LocalObra} — {obra.Cidade}");
                    col.Item().Text($"Número RT: {obra.NumeroRt}");
                    col.Item().Text($"Tipo de edificação: {obra.TipoEdificacao}");
                    col.Item().Text($"Área total edificada: {obra.AreaTotalEdificadaM2} m²");
                    col.Item().Text($"Valor da obra: {obra.ValorObra:C}");
                    col.Item().Text($"Status: {obra.Status}");

                    col.Item().PaddingTop(10).Text($"Registros de visita ({registros.Count})").SemiBold();
                    foreach (var r in registros)
                    {
                        col.Item().Text($"- {r.DataVisita:dd/MM/yyyy} — {r.PosicaoObra} — {r.Status}");
                    }

                    if (termo is not null)
                    {
                        col.Item().PaddingTop(10).Text("Termo de conclusão").SemiBold();
                        col.Item().Text($"Concluída em {termo.DataConclusao:dd/MM/yyyy} — {termo.Declaracao}");
                    }
                });

                page.Footer().AlignCenter().Text("Documento gerado eletronicamente — Caderneta de Obras Digital / CREA");
            });
        }).GeneratePdf();
    }
}
