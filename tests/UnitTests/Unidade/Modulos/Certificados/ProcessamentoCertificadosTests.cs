using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Certificados;

[TestClass]
public sealed class ProcessamentoCertificadosTests
{
    [TestMethod]
    public void DeveCriarUmCertificadoParaCadaAluno()
    {
        ProcessamentoCertificados processamento =
            ProcessamentoCertificados.Criar(Guid.NewGuid(), Guid.NewGuid());

        processamento.Certificados.Add(
            Certificado.Criar(
                processamento.Id,
                processamento.CursoId,
                processamento.UsuarioId,
                "Ana"
            )
        );

        Assert.AreEqual(1, processamento.Certificados.Count);
        Assert.AreEqual(StatusCertificado.Pendente, processamento.Certificados.Single().Status);
    }

    [TestMethod]
    public void DeveRejeitarNomeDeAlunoInvalido()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Certificado.Criar(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), " ")
        );
    }

    [TestMethod]
    public void DeveConcluirComCaminhoDoZip()
    {
        ProcessamentoCertificados processamento =
            ProcessamentoCertificados.Criar(Guid.NewGuid(), Guid.NewGuid());

        processamento.IniciarCertificados();
        processamento.IniciarZip();
        processamento.Concluir("zips/usuario/processamento.zip");

        Assert.AreEqual(StatusProcessamento.Concluido, processamento.Status);
        Assert.IsTrue(processamento.Finalizado);
        Assert.AreEqual(
            "zips/usuario/processamento.zip",
            processamento.CaminhoZip
        );
    }
}
