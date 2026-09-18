using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using GeradorCertificadosOnline.Infraestrutura.Modulos.Cursos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.IntegrationTests.Modulos;

[TestClass]
public sealed class RepositorioCursoEmOrmTests
{
    private SqliteConnection conexao = null!;
    private GeradorCertificadosOnlineDbContext contexto = null!;
    private RepositorioCursoEmOrm repositorio = null!;

    [TestInitialize]
    public async Task InicializarAsync()
    {
        conexao = new SqliteConnection("DataSource=:memory:");
        await conexao.OpenAsync();

        var opcoes = new DbContextOptionsBuilder<GeradorCertificadosOnlineDbContext>()
            .UseSqlite(conexao)
            .Options;

        contexto = new GeradorCertificadosOnlineDbContext(opcoes);
        await contexto.Database.EnsureCreatedAsync();
        repositorio = new RepositorioCursoEmOrm(contexto);
    }

    [TestCleanup]
    public async Task FinalizarAsync()
    {
        await contexto.DisposeAsync();
        await conexao.DisposeAsync();
    }

    [TestMethod]
    public async Task DeveCadastrarERecuperarCursoPorIdPreservandoOsDados()
    {
        Curso curso = Curso.Criar(
            "Introdução ao C#",
            "Fundamentos de C# e .NET.",
            40,
            new DateOnly(2026, 9, 16)
        );

        await repositorio.CadastrarAsync(curso);

        Curso? cursoRecuperado = await repositorio.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(cursoRecuperado);
        Assert.AreEqual(curso.Id, cursoRecuperado.Id);
        Assert.AreEqual("Introdução ao C#", cursoRecuperado.Nome);
        Assert.AreEqual("Fundamentos de C# e .NET.", cursoRecuperado.Descricao);
        Assert.AreEqual(40, cursoRecuperado.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 9, 16), cursoRecuperado.DataConclusao);
    }

    [TestMethod]
    public async Task DeveCadastrarERecuperarCursoComDescricaoNula()
    {
        Curso curso = Curso.Criar("Curso sem descrição", null, 20, new DateOnly(2026, 1, 1));

        await repositorio.CadastrarAsync(curso);

        Curso? cursoRecuperado = await repositorio.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(cursoRecuperado);
        Assert.IsNull(cursoRecuperado.Descricao);
    }
}
