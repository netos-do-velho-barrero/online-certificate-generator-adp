namespace GeradorCertificadosOnline.Aplicacao.Compartilhado.Contratos.Cursos;

public interface IConsultaCurso
{
    Task<CursoResumo?> ObterPorIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );
}
