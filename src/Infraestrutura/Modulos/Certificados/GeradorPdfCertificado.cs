using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class GeradorPdfCertificado : IGeradorPdfCertificado
{
    public byte[] Gerar(
        string nomeAluno,
        string nomeCurso,
        int cargaHoraria,
        DateOnly dataEmissao
    )
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(60);
                    page.PageColor(Colors.White);
                    page.Content().Column(column =>
                    {
                        column.Spacing(24);
                        column.Item().AlignCenter().Text("CERTIFICADO")
                            .FontSize(30).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().AlignCenter().Text($"Certificamos que {nomeAluno}")
                            .FontSize(20);
                        column.Item().AlignCenter().Text($"Concluiu o curso {nomeCurso}")
                            .FontSize(18);
                        column.Item().AlignCenter().Text($"Carga Horária: {cargaHoraria} horas")
                            .FontSize(16);
                        column.Item().AlignCenter().Text($"Data: {dataEmissao:dd/MM/yyyy}")
                            .FontSize(16);
                    });
                })
            )
            .GeneratePdf();
    }
}
