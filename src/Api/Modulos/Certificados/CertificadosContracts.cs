using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;

namespace GeradorCertificadosOnline.Api.Modulos.Certificados;

public sealed record SolicitarCertificadosHttpRequest(List<AlunoRequest> Alunos);