using GeradorCertificadosOnline.Api.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.IntegrationTests.Modulos;

[TestClass]
public sealed class CursosControllerTests
{
    [TestMethod]
    public async Task CadastrarDeveEnviarCadastrarCursoCommandComOsDadosDoRequest()
    {
        SenderFake sender = new()
        {
            ResponderCom = _ => new CursoDto(Guid.NewGuid(), "Curso Teste", "Descrição", 40, new DateOnly(2026, 9, 16))
        };
        CursosController controller = new(sender);
        CadastroCursoRequest request = new("Curso Teste", "Descrição", 40, new DateOnly(2026, 9, 16));

        await controller.Cadastrar(request, CancellationToken.None);

        Assert.IsInstanceOfType<CadastrarCursoCommand>(sender.UltimaRequisicao);
        CadastrarCursoCommand command = (CadastrarCursoCommand)sender.UltimaRequisicao!;
        Assert.AreEqual("Curso Teste", command.Nome);
        Assert.AreEqual("Descrição", command.Descricao);
        Assert.AreEqual(40, command.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), command.DataConclusao);
    }

    [TestMethod]
    public async Task CadastrarDeveRetornar201CreatedComDadosCorretosELocationParaObterPorId()
    {
        Guid cursoId = Guid.NewGuid();
        SenderFake sender = new()
        {
            ResponderCom = _ => new CursoDto(cursoId, "Curso Teste", "Descrição", 40, new DateOnly(2026, 9, 16))
        };
        CursosController controller = new(sender);
        CadastroCursoRequest request = new("Curso Teste", "Descrição", 40, new DateOnly(2026, 9, 16));

        IActionResult resultado = await controller.Cadastrar(request, CancellationToken.None);

        Assert.IsInstanceOfType<CreatedAtActionResult>(resultado);
        CreatedAtActionResult created = (CreatedAtActionResult)resultado;
        Assert.AreEqual(StatusCodes.Status201Created, created.StatusCode);
        Assert.AreEqual(nameof(CursosController.ObterPorId), created.ActionName);
        Assert.IsNotNull(created.RouteValues);
        Assert.AreEqual(cursoId, created.RouteValues["cursoId"]);

        Assert.IsInstanceOfType<CursoResponse>(created.Value);
        CursoResponse response = (CursoResponse)created.Value!;
        Assert.AreEqual(cursoId, response.Id);
        Assert.AreEqual("Curso Teste", response.Nome);
        Assert.AreEqual("Descrição", response.Descricao);
        Assert.AreEqual(40, response.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), response.DataConclusao);
    }

    [TestMethod]
    public async Task ObterPorIdDeveEnviarQueryComOCursoIdDaRota()
    {
        Guid cursoId = Guid.NewGuid();
        SenderFake sender = new()
        {
            ResponderCom = _ => new CursoDto(cursoId, "Curso Teste", null, 40, new DateOnly(2026, 9, 16))
        };
        CursosController controller = new(sender);

        await controller.ObterPorId(cursoId, CancellationToken.None);

        Assert.IsInstanceOfType<ObterCursoPorIdQuery>(sender.UltimaRequisicao);
        ObterCursoPorIdQuery query = (ObterCursoPorIdQuery)sender.UltimaRequisicao!;
        Assert.AreEqual(cursoId, query.CursoId);
    }

    [TestMethod]
    public async Task ObterPorIdDeveRetornar200ComDadosCorretos()
    {
        Guid cursoId = Guid.NewGuid();
        SenderFake sender = new()
        {
            ResponderCom = _ => new CursoDto(cursoId, "Curso Teste", null, 40, new DateOnly(2026, 9, 16))
        };
        CursosController controller = new(sender);

        ActionResult<CursoResponse> resultado = await controller.ObterPorId(cursoId, CancellationToken.None);

        Assert.IsInstanceOfType<OkObjectResult>(resultado.Result);
        OkObjectResult ok = (OkObjectResult)resultado.Result!;
        Assert.AreEqual(StatusCodes.Status200OK, ok.StatusCode);

        Assert.IsInstanceOfType<CursoResponse>(ok.Value);
        CursoResponse response = (CursoResponse)ok.Value!;
        Assert.AreEqual(cursoId, response.Id);
        Assert.AreEqual("Curso Teste", response.Nome);
        Assert.IsNull(response.Descricao);
        Assert.AreEqual(40, response.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), response.DataConclusao);
    }

    private sealed class SenderFake : ISender
    {
        public object? UltimaRequisicao { get; private set; }

        public Func<object, object?>? ResponderCom { get; set; }

        public Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            UltimaRequisicao = request;
            return Task.FromResult((TResponse)ResponderCom!.Invoke(request)!);
        }

        public Task Send<TRequest>(
            TRequest request,
            CancellationToken cancellationToken = default
        ) where TRequest : IRequest
        {
            UltimaRequisicao = request;
            ResponderCom?.Invoke(request!);
            return Task.CompletedTask;
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            UltimaRequisicao = request;
            return Task.FromResult(ResponderCom?.Invoke(request));
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default
        ) => throw new NotSupportedException();

        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default
        ) => throw new NotSupportedException();
    }
}
