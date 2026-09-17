using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Usuarios;

[TestClass]
public sealed class UsuarioTests
{
    [TestMethod]
    public void CriarDeveNormalizarEmailERegistrarAuditoria()
    {
        DateTime agoraUtc = new(2026, 9, 17, 15, 0, 0, DateTimeKind.Utc);

        Usuario usuario = Usuario.Criar(
            "  ALUNO@EXEMPLO.COM ",
            "AQAAAAIAAYagAAAAEhash",
            agoraUtc: agoraUtc
        );

        Assert.AreEqual("aluno@exemplo.com", usuario.Email);
        Assert.AreEqual(agoraUtc, usuario.DataCriacaoEmUtc);
        Assert.AreEqual(agoraUtc, usuario.DataAtualizacaoEmUtc);
        Assert.AreEqual(0, usuario.Validar().Count);
    }

    [TestMethod]
    public void AlterarSenhaDeveAtualizarHashEData()
    {
        DateTime criacaoUtc = new(2026, 9, 17, 15, 0, 0, DateTimeKind.Utc);
        DateTime atualizacaoUtc = criacaoUtc.AddMinutes(1);
        Usuario usuario = Usuario.Criar(
            "aluno@exemplo.com",
            "hash-antigo",
            agoraUtc: criacaoUtc
        );

        usuario.AlterarSenhaHash("hash-novo", atualizacaoUtc);

        Assert.AreEqual("hash-novo", usuario.SenhaHash);
        Assert.AreEqual(atualizacaoUtc, usuario.DataAtualizacaoEmUtc);
        Assert.AreEqual(criacaoUtc, usuario.DataCriacaoEmUtc);
    }

    [TestMethod]
    public void CriarDeveRejeitarEmailInvalido()
    {
        Assert.Throws<ArgumentException>(() =>
            Usuario.Criar("email-invalido", "hash")
        );
    }
}