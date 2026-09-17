using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class ControllerBaseExtensions
{
    public static ActionResult ProblemFromException(
        this ControllerBase controller,
        Exception excecao
    )
    {
        return excecao switch
        {
            ValidacaoDeIdentidadeException validacao => controller.BadRequest(
                CriarProblem(
                    400,
                    "Erro de validação",
                    validacao.Message,
                    ProblemDetailsTypes.Validacao,
                    new Dictionary<string, object?> { ["campo"] = validacao.Campo }
                )
            ),
            ConflitoDeIdentidadeException => controller.Conflict(
                CriarProblem(409, "Conflito", excecao.Message, ProblemDetailsTypes.Conflito)
            ),
            KeyNotFoundException => controller.NotFound(
                CriarProblem(404, "Recurso não encontrado", excecao.Message, ProblemDetailsTypes.NaoEncontrado)
            ),
            UnauthorizedAccessException => controller.Unauthorized(
                CriarProblem(401, "Não autenticado", excecao.Message, ProblemDetailsTypes.NaoAutenticado)
            ),
            _ => controller.StatusCode(
                500,
                CriarProblem(500, "Erro interno", "Ocorreu um erro interno.", ProblemDetailsTypes.ErroInterno)
            )
        };
    }

    private static ProblemDetails CriarProblem(
        int status,
        string titulo,
        string detalhe,
        string tipo,
        IDictionary<string, object?>? extensoes = null
    )
    {
        ProblemDetails problem = new()
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Type = tipo
        };

        if (extensoes is not null)
        {
            foreach (var extensao in extensoes)
            {
                problem.Extensions[extensao.Key] = extensao.Value;
            }
        }

        return problem;
    }
}