using FluentValidation;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed record CadastrarCursoCommand(
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateOnly DataConclusao
) : IRequest<CursoDto>;

public sealed class CadastrarCursoCommandValidator : AbstractValidator<CadastrarCursoCommand>
{
    public CadastrarCursoCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(500);
        RuleFor(x => x.CargaHoraria).GreaterThan(0);
        RuleFor(x => x.DataConclusao).NotEmpty();
    }
}

public sealed class CadastrarCursoCommandHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<CadastrarCursoCommand, CursoDto>
{
    public async Task<CursoDto> Handle(
        CadastrarCursoCommand request,
        CancellationToken cancellationToken
    )
    {
        Curso curso = Curso.Criar(
            request.Nome,
            request.Descricao,
            request.CargaHoraria,
            request.DataConclusao
        );

        await repositorioCurso.CadastrarAsync(curso, cancellationToken);

        return Converter(curso);
    }

    public static CursoDto Converter(Curso curso) =>
        new(curso.Id, curso.Nome, curso.Descricao, curso.CargaHoraria, curso.DataConclusao);
}
