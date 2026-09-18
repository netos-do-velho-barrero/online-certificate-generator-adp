using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed record ObterCursoPorIdQuery(Guid CursoId) : IRequest<CursoDto>;

public sealed class ObterCursoPorIdQueryHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<ObterCursoPorIdQuery, CursoDto>
{
    public async Task<CursoDto> Handle(
        ObterCursoPorIdQuery request,
        CancellationToken cancellationToken
    )
    {
        Curso curso = await repositorioCurso.SelecionarPorIdAsync(
            request.CursoId,
            cancellationToken
        ) ?? throw new KeyNotFoundException(ErrosDeCursos.NaoEncontrado);

        return CadastrarCursoCommandHandler.Converter(curso);
    }
}
