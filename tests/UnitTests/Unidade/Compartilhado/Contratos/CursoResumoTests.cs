using GeradorCertificadosOnline.Aplicacao.Compartilhado.Contratos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Compartilhado.Contratos;

[TestClass]
public sealed class CursoResumoTests
{
    [TestMethod]
    public void DevePreservarOsDadosDoResumoDoCurso()
    {
        Guid id = Guid.NewGuid();
        DateOnly dataConclusao = new(2026, 9, 17);

        CursoResumo resumo = new(id, "C# Essencial", 40, dataConclusao);

        Assert.AreEqual(id, resumo.Id);
        Assert.AreEqual("C# Essencial", resumo.Nome);
        Assert.AreEqual(40, resumo.CargaHoraria);
        Assert.AreEqual(dataConclusao, resumo.DataConclusao);
    }
}
