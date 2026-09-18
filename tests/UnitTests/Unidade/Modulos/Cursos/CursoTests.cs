using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Cursos;

[TestClass]
public sealed class CursoTests
{
    [TestMethod]
    public void CriarDeveArmazenarOsDadosDoCurso()
    {
        DateOnly dataConclusao = new(2026, 9, 16);

        Curso curso = Curso.Criar(
            "Introdução ao C#",
            "Fundamentos de C# e .NET.",
            40,
            dataConclusao
        );

        Assert.AreEqual("Introdução ao C#", curso.Nome);
        Assert.AreEqual("Fundamentos de C# e .NET.", curso.Descricao);
        Assert.AreEqual(40, curso.CargaHoraria);
        Assert.AreEqual(dataConclusao, curso.DataConclusao);
        Assert.AreEqual(0, curso.Validar().Count);
    }

    [TestMethod]
    public void CriarDeveRejeitarNomeNulo()
    {
        Assert.Throws<ArgumentException>(() =>
            Curso.Criar(null!, null, 40, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveRejeitarNomeVazio()
    {
        Assert.Throws<ArgumentException>(() =>
            Curso.Criar(string.Empty, null, 40, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveRejeitarNomeEmBranco()
    {
        Assert.Throws<ArgumentException>(() =>
            Curso.Criar("   ", null, 40, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveRejeitarNomeComMaisDe200Caracteres()
    {
        string nomeInvalido = new('a', 201);

        Assert.Throws<ArgumentException>(() =>
            Curso.Criar(nomeInvalido, null, 40, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDevePermitirDescricaoNaoInformada()
    {
        Curso curso = Curso.Criar("Curso sem descrição", null, 40, new DateOnly(2026, 9, 16));

        Assert.IsNull(curso.Descricao);
        Assert.AreEqual(0, curso.Validar().Count);
    }

    [TestMethod]
    public void CriarDeveRejeitarDescricaoComMaisDe500Caracteres()
    {
        string descricaoInvalida = new('a', 501);

        Assert.Throws<ArgumentException>(() =>
            Curso.Criar("Curso", descricaoInvalida, 40, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveRejeitarCargaHorariaIgualAZero()
    {
        Assert.Throws<ArgumentException>(() =>
            Curso.Criar("Curso", null, 0, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveRejeitarCargaHorariaNegativa()
    {
        Assert.Throws<ArgumentException>(() =>
            Curso.Criar("Curso", null, -10, new DateOnly(2026, 9, 16))
        );
    }

    [TestMethod]
    public void CriarDeveArmazenarDataConclusaoCorretamente()
    {
        DateOnly dataConclusao = new(2026, 12, 1);

        Curso curso = Curso.Criar("Curso", null, 40, dataConclusao);

        Assert.AreEqual(dataConclusao, curso.DataConclusao);
    }

    [TestMethod]
    public void AtualizarDeveSubstituirOsDadosDoCurso()
    {
        Curso curso = Curso.Criar(
            "Curso original",
            "Descrição original",
            20,
            new DateOnly(2026, 1, 1)
        );
        Curso cursoAtualizado = Curso.Criar(
            "Curso atualizado",
            "Descrição atualizada",
            60,
            new DateOnly(2026, 6, 1)
        );

        curso.Atualizar(cursoAtualizado);

        Assert.AreEqual("Curso atualizado", curso.Nome);
        Assert.AreEqual("Descrição atualizada", curso.Descricao);
        Assert.AreEqual(60, curso.CargaHoraria);
        Assert.AreEqual(new DateOnly(2026, 6, 1), curso.DataConclusao);
    }

    [TestMethod]
    public void AtualizarDeveRejeitarEntidadeNula()
    {
        Curso curso = Curso.Criar("Curso original", null, 20, new DateOnly(2026, 1, 1));

        Assert.Throws<ArgumentNullException>(() =>
            curso.Atualizar(null!)
        );
    }
}
