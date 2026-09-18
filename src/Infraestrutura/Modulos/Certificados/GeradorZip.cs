using System.IO.Compression;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class GeradorZip : IGeradorZip
{
    public byte[] Gerar(IReadOnlyList<(string NomeArquivo, byte[] Conteudo)> arquivos)
    {
        using MemoryStream stream = new();
        using (ZipArchive archive = new(stream, ZipArchiveMode.Create, true))
        {
            foreach ((string nomeArquivo, byte[] conteudo) in arquivos)
            {
                ZipArchiveEntry entry = archive.CreateEntry(
                    $"certificados/{nomeArquivo}",
                    CompressionLevel.Fastest
                );
                using Stream entryStream = entry.Open();
                entryStream.Write(conteudo);
            }
        }

        return stream.ToArray();
    }
}
