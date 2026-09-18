using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class ArmazenamentoArquivosOptions
{
    public string RootPath { get; set; } = "storage";
}

public sealed class ArmazenamentoArquivosLocal(
    ArmazenamentoArquivosOptions options
) : IArmazenamentoArquivos
{
    private readonly string _root = Path.GetFullPath(options.RootPath);

    public async Task<string> SalvarAsync(
        string pasta,
        string nomeArquivo,
        byte[] conteudo,
        CancellationToken ct = default
    )
    {
        Directory.CreateDirectory(_root);
        string caminhoRelativo = Path.Combine(pasta, nomeArquivo);
        string caminho = ObterCaminhoSeguro(caminhoRelativo);
        Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
        await File.WriteAllBytesAsync(caminho, conteudo, ct);
        return caminhoRelativo.Replace('\\', '/');
    }

    public Task<byte[]> LerAsync(string caminho, CancellationToken ct = default)
    {
        string caminhoSeguro = ObterCaminhoSeguro(caminho);
        return File.ReadAllBytesAsync(caminhoSeguro, ct);
    }

    private string ObterCaminhoSeguro(string caminhoRelativo)
    {
        string caminho = Path.GetFullPath(Path.Combine(_root, caminhoRelativo));
        if (!caminho.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(caminho, _root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de arquivo inválido.");
        }

        return caminho;
    }
}
