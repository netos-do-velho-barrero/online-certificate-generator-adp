using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Cursos;

[TestClass]
public sealed class ObterCursoPorIdQueryHandlerTests
{
    [TestMethod]
    public async Task HandleDeveRetornarCursoExistente()
    {
        Curso curso = Curso.Criar("Curso existente", "Descrição", 40, new DateOnly(2026, 9, 16));
        RepositorioCursoFake repositorio = new();
        await repositorio.CadastrarAsync(curso);
        ObterCursoPorIdQueryHandler handler = new(repositorio);

        CursoDto resultado = await handler.Handle(
            new ObterCursoPorIdQuery(curso.Id),
            CancellationToken.None
        );

        Assert.AreEqual(curso.Id, resultado.Id);
        Assert.AreEqual("Curso existente", resultado.Nome);
        Assert.AreEqual("Descrição", resultado.Descricao);
        Assert.AreEqual(40, resultado.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), resultado.DataConclusao);
    }

    [TestMethod]
    public async Task HandleDeveLancarKeyNotFoundExceptionQuandoCursoNaoExiste()
    {
        RepositorioCursoFake repositorio = new();
        ObterCursoPorIdQueryHandler handler = new(repositorio);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(() =>
            handler.Handle(new ObterCursoPorIdQuery(Guid.NewGuid()), CancellationToken.None)
        );
    }

    private sealed class RepositorioCursoFake : IRepositorioCurso
    {
        private readonly Dictionary<Guid, Curso> cursos = [];

        public Task CadastrarAsync(Curso entidade, CancellationToken cancellationToken = default)
        {
            cursos[entidade.Id] = entidade;
            return Task.CompletedTask;
        }

        public Task<bool> EditarAsync(
            Guid idSelecionado,
            Curso entidadeAtualizada,
            CancellationToken cancellationToken = default
        ) => throw new NotSupportedException();

        public Task<bool> ExcluirAsync(
            Guid idSelecionado,
            CancellationToken cancellationToken = default
        ) => throw new NotSupportedException();

        public Task<Curso?> SelecionarPorIdAsync(
            Guid idSelecionado,
            CancellationToken cancellationToken = default
        )
        {
            cursos.TryGetValue(idSelecionado, out Curso? curso);
            return Task.FromResult(curso);
        }

        public Task<List<Curso>> SelecionarTodosAsync(
            CancellationToken cancellationToken = default
        ) => Task.FromResult(cursos.Values.ToList());
    }
}
