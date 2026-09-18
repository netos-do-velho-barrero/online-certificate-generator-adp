namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
public sealed record AlunoRequest(string Nome);
public sealed record SolicitarCertificadosRequest(List<AlunoRequest> Alunos);
public sealed record ProcessamentoResponse(Guid ProcessamentoId, string Status);
public sealed record CertificadoResponse(Guid Id, string NomeAluno, string Status, DateTime? GeradoEm);
public sealed record StatusResponse(Guid ProcessamentoId, string Status, int Total, int Gerados, int Falhas, string? Download);
public sealed record ArquivoDownloadResponse(byte[] Conteudo, string NomeArquivo);
