using FluentValidation.Results;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Cursos;

[TestClass]
public sealed class CadastrarCursoCommandHandlerTests
{
    [TestMethod]
    public async Task HandleDeveCadastrarCursoValidoERetornarDadosCorretos()
    {
        RepositorioCursoFake repositorio = new();
        CadastrarCursoCommandHandler handler = new(repositorio);
        CadastrarCursoCommand command = new(
            "Introdução ao C#",
            "Fundamentos de C# e .NET.",
            40,
            new DateOnly(2026, 9, 16)
        );

        CursoDto resultado = await handler.Handle(command, CancellationToken.None);

        Assert.IsNotNull(repositorio.CursoCadastrado);
        Assert.AreEqual(resultado.Id, repositorio.CursoCadastrado.Id);
        Assert.AreEqual("Introdução ao C#", resultado.Nome);
        Assert.AreEqual("Fundamentos de C# e .NET.", resultado.Descricao);
        Assert.AreEqual(40, resultado.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), resultado.DataConclusao);
    }

    [TestMethod]
    public void ValidatorDeveRejeitarNomeVazio()
    {
        CadastrarCursoCommandValidator validator = new();
        CadastrarCursoCommand command = new(string.Empty, null, 40, new DateOnly(2026, 9, 16));

        ValidationResult resultado = validator.Validate(command);

        Assert.IsFalse(resultado.IsValid);
        Assert.IsTrue(resultado.Errors.Exists(e => e.PropertyName == nameof(CadastrarCursoCommand.Nome)));
    }

    [TestMethod]
    public void ValidatorDeveRejeitarNomeComMaisDe200Caracteres()
    {
        CadastrarCursoCommandValidator validator = new();
        string nomeInvalido = new('a', 201);
        CadastrarCursoCommand command = new(nomeInvalido, null, 40, new DateOnly(2026, 9, 16));

        ValidationResult resultado = validator.Validate(command);

        Assert.IsFalse(resultado.IsValid);
        Assert.IsTrue(resultado.Errors.Exists(e => e.PropertyName == nameof(CadastrarCursoCommand.Nome)));
    }

    [TestMethod]
    public void ValidatorDevePermitirDescricaoNaoInformada()
    {
        CadastrarCursoCommandValidator validator = new();
        CadastrarCursoCommand command = new("Curso", null, 40, new DateOnly(2026, 9, 16));

        ValidationResult resultado = validator.Validate(command);

        Assert.IsTrue(resultado.IsValid);
    }

    [TestMethod]
    public void ValidatorDeveRejeitarDescricaoComMaisDe500Caracteres()
    {
        CadastrarCursoCommandValidator validator = new();
        string descricaoInvalida = new('a', 501);
        CadastrarCursoCommand command = new("Curso", descricaoInvalida, 40, new DateOnly(2026, 9, 16));

        ValidationResult resultado = validator.Validate(command);

        Assert.IsFalse(resultado.IsValid);
        Assert.IsTrue(resultado.Errors.Exists(e => e.PropertyName == nameof(CadastrarCursoCommand.Descricao)));
    }

    [TestMethod]
    public void ValidatorDeveRejeitarCargaHorariaMenorOuIgualAZero()
    {
        CadastrarCursoCommandValidator validator = new();
        CadastrarCursoCommand command = new("Curso", null, 0, new DateOnly(2026, 9, 16));

        ValidationResult resultado = validator.Validate(command);

        Assert.IsFalse(resultado.IsValid);
        Assert.IsTrue(resultado.Errors.Exists(e => e.PropertyName == nameof(CadastrarCursoCommand.CargaHoraria)));
    }

    [TestMethod]
    public void ValidatorDeveRejeitarDataConclusaoNaoInformada()
    {
        CadastrarCursoCommandValidator validator = new();
        CadastrarCursoCommand command = new("Curso", null, 40, default);

        ValidationResult resultado = validator.Validate(command);

        Assert.IsFalse(resultado.IsValid);
        Assert.IsTrue(resultado.Errors.Exists(e => e.PropertyName == nameof(CadastrarCursoCommand.DataConclusao)));
    }

    private sealed class RepositorioCursoFake : IRepositorioCurso
    {
        private readonly Dictionary<Guid, Curso> cursos = [];

        public Curso? CursoCadastrado { get; private set; }

        public Task CadastrarAsync(Curso entidade, CancellationToken cancellationToken = default)
        {
            cursos[entidade.Id] = entidade;
            CursoCadastrado = entidade;
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
