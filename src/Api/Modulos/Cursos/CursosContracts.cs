namespace GeradorCertificadosOnline.Api.Modulos.Cursos;

public sealed record CadastroCursoRequest(
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateOnly DataConclusao
);

public sealed record CursoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateOnly DataConclusao
);
