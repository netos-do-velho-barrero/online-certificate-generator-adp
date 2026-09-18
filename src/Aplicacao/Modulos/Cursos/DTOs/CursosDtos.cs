namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;

public sealed record CursoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateOnly DataConclusao
);
