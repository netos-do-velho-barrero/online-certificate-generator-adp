using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Cursos;

[ApiController]
[Route("cursos")]
public sealed class CursosController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Cadastrar(
        CadastroCursoRequest request,
        CancellationToken cancellationToken
    )
    {
        CursoDto curso = await sender.Send(
            new CadastrarCursoCommand(
                request.Nome,
                request.Descricao,
                request.CargaHoraria,
                request.DataConclusao
            ),
            cancellationToken
        );

        CursoResponse response = Converter(curso);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { cursoId = response.Id },
            response
        );
    }

    [Authorize]
    [HttpGet("{cursoId}")]
    [ProducesResponseType(typeof(CursoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursoResponse>> ObterPorId(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        CursoDto curso = await sender.Send(
            new ObterCursoPorIdQuery(cursoId),
            cancellationToken
        );

        return Ok(Converter(curso));
    }

    private static CursoResponse Converter(CursoDto curso) =>
        new(curso.Id, curso.Nome, curso.Descricao, curso.CargaHoraria, curso.DataConclusao);
}
