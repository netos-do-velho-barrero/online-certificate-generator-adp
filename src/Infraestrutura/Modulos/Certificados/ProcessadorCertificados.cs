using GeradorCertificadosOnline.Aplicacao.Compartilhado.Contratos.Cursos;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.Extensions.Logging;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class ProcessadorCertificados(
    IRepositorioProcessamento repositorio,
    IConsultaCurso consultaCurso,
    IArmazenamentoArquivos armazenamento,
    IGeradorPdfCertificado geradorPdf,
    IGeradorZip geradorZip,
    ILogger<ProcessadorCertificados> logger
) : IProcessadorCertificados
{
    public async Task ProcessarAsync(Guid processamentoId, CancellationToken ct = default)
    {
        ProcessamentoCertificados? processamento =
            await repositorio.ObterPorIdComCertificadosAsync(processamentoId, null, true, ct);

        if (processamento is null)
        {
            throw new KeyNotFoundException("Processamento de certificados não encontrado.");
        }

        if (processamento.Finalizado)
        {
            return;
        }

        CursoResumo? curso = await consultaCurso.ObterPorIdAsync(processamento.CursoId, ct)
            ?? throw new KeyNotFoundException("Curso não encontrado.");

        processamento.IniciarCertificados();
        await repositorio.PersistirAsync(ct);

        foreach (Certificado certificado in processamento.Certificados)
        {
            if (certificado.Status == StatusCertificado.Gerado)
            {
                continue;
            }

            try
            {
                certificado.Iniciar();
                byte[] pdf = geradorPdf.Gerar(
                    certificado.NomeAluno,
                    curso.Nome,
                    curso.CargaHoraria,
                    curso.DataConclusao
                );
                string caminho = await armazenamento.SalvarAsync(
                    $"certificados/{processamento.UsuarioId}/{processamento.Id}",
                    $"{certificado.Id}.pdf",
                    pdf,
                    ct
                );
                certificado.Gerar(caminho);
            }
            catch (Exception excecao) when (excecao is not OperationCanceledException)
            {
                logger.LogError(excecao, "Falha ao gerar certificado {CertificadoId}.", certificado.Id);
                certificado.Falhar(excecao.Message);
            }

            await repositorio.PersistirAsync(ct);
        }

        if (processamento.Certificados.Any(x => x.Status == StatusCertificado.Falha))
        {
            processamento.Falhar();
            await repositorio.PersistirAsync(ct);
            return;
        }

        processamento.IniciarZip();
        await repositorio.PersistirAsync(ct);

        List<(string NomeArquivo, byte[] Conteudo)> arquivos = [];
        foreach (Certificado certificado in processamento.Certificados)
        {
            byte[] conteudo = await armazenamento.LerAsync(certificado.CaminhoPdf!, ct);
            arquivos.Add(($"{NomeArquivoSeguro(certificado.NomeAluno)}-{certificado.Id}.pdf", conteudo));
        }

        byte[] zip = geradorZip.Gerar(arquivos);
        string caminhoZip = await armazenamento.SalvarAsync(
            $"zips/{processamento.UsuarioId}",
            $"{processamento.Id}.zip",
            zip,
            ct
        );
        processamento.Concluir(caminhoZip);
        await repositorio.PersistirAsync(ct);
    }

    private static string NomeArquivoSeguro(string nome)
    {
        char[] invalidos = Path.GetInvalidFileNameChars();
        return string.Concat(nome.Select(caractere =>
            invalidos.Contains(caractere) ? '_' : caractere));
    }
}
