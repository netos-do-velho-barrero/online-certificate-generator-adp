using System.IO.Compression;
using GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Certificados;

[TestClass]
public sealed class GeradoresCertificadosTests
{
    [TestMethod]
    public void DeveGerarPdfComConteudoPdf()
    {
        GeradorPdfCertificado gerador = new();

        byte[] pdf = gerador.Gerar(
            "Ana Souza",
            "Introdução ao C#",
            40,
            new DateOnly(2026, 9, 18)
        );

        Assert.IsTrue(pdf.Length > 4);
        Assert.AreEqual("%PDF", System.Text.Encoding.ASCII.GetString(pdf, 0, 4));
    }

    [TestMethod]
    public void DeveCriarZipComUmArquivoPorCertificado()
    {
        GeradorZip gerador = new();

        byte[] zip = gerador.Gerar(
        [
            ("ana.pdf", "%PDF-ana"u8.ToArray()),
            ("bruno.pdf", "%PDF-bruno"u8.ToArray())
        ]);

        using MemoryStream stream = new(zip);
        using ZipArchive archive = new(stream, ZipArchiveMode.Read);

        CollectionAssert.AreEquivalent(
            new[] { "certificados/ana.pdf", "certificados/bruno.pdf" },
            archive.Entries.Select(entry => entry.FullName).ToArray()
        );
    }
}
