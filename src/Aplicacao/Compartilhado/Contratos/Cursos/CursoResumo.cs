namespace GeradorCertificadosOnline.Aplicacao.Compartilhado.Contratos.Cursos;

public sealed record CursoResumo(
    Guid Id,
    string Nome,
    int CargaHoraria,
    DateOnly DataConclusao
);
