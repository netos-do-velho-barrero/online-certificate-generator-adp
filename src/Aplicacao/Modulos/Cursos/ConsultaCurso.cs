using GeradorCertificadosOnline.Aplicacao.Compartilhado.Contratos.Cursos;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed class ConsultaCurso(
    IRepositorioCurso repositorioCurso
) : IConsultaCurso
{
    public async Task<CursoResumo?> ObterPorIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(cursoId, cancellationToken);

        return curso is null
            ? null
            : new CursoResumo(curso.Id, curso.Nome, curso.CargaHoraria, curso.DataConclusao);
    }
}
