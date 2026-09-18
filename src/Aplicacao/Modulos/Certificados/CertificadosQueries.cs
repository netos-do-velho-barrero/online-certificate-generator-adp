using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
public sealed record ObterStatusCertificadosQuery(Guid CursoId, Guid UsuarioId) : IRequest<StatusResponse>;
public sealed record ListarCertificadosQuery(Guid CursoId, Guid UsuarioId) : IRequest<IReadOnlyList<CertificadoResponse>>;
public sealed record DownloadCertificadosQuery(Guid CursoId, Guid UsuarioId) : IRequest<ArquivoDownloadResponse>;
public sealed class ObterStatusHandler(IRepositorioProcessamento repo) : IRequestHandler<ObterStatusCertificadosQuery, StatusResponse>
{
 public async Task<StatusResponse> Handle(ObterStatusCertificadosQuery r, CancellationToken ct) { var p=await repo.ObterComCertificadosAsync(r.CursoId,r.UsuarioId,ct) ?? throw new KeyNotFoundException("Processamento não encontrado."); return new(p.Id,p.Status.ToString(),p.Certificados.Count,p.Certificados.Count(x=>x.Status==StatusCertificado.Gerado),p.Certificados.Count(x=>x.Status==StatusCertificado.Falha),p.Status==StatusProcessamento.Concluido?"/cursos/"+r.CursoId+"/certificados/download":null); }
}

public sealed class DownloadCertificadosHandler(
    IRepositorioProcessamento repo,
    IArmazenamentoArquivos armazenamento
) : IRequestHandler<DownloadCertificadosQuery, ArquivoDownloadResponse>
{
    public async Task<ArquivoDownloadResponse> Handle(
        DownloadCertificadosQuery r,
        CancellationToken ct
    )
    {
        ProcessamentoCertificados processamento =
            await repo.ObterComCertificadosAsync(r.CursoId, r.UsuarioId, ct)
            ?? throw new KeyNotFoundException("Processamento não encontrado.");

        if (processamento.Status != StatusProcessamento.Concluido ||
            string.IsNullOrWhiteSpace(processamento.CaminhoZip))
        {
            throw new InvalidOperationException("O processamento ainda não foi concluído.");
        }

        byte[] conteudo = await armazenamento.LerAsync(processamento.CaminhoZip, ct);
        return new(conteudo, $"certificados-{r.CursoId}.zip");
    }
}
public sealed class ListarCertificadosHandler(IRepositorioProcessamento repo) : IRequestHandler<ListarCertificadosQuery, IReadOnlyList<CertificadoResponse>>
{
 public async Task<IReadOnlyList<CertificadoResponse>> Handle(ListarCertificadosQuery r, CancellationToken ct) { var p=await repo.ObterComCertificadosAsync(r.CursoId,r.UsuarioId,ct) ?? throw new KeyNotFoundException("Processamento não encontrado."); return p.Certificados.Select(x=>new CertificadoResponse(x.Id,x.NomeAluno,x.Status.ToString(),x.GeradoEm)).ToList(); }
}
