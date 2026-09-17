using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.UnitTests.Unidade.Modulos.Usuarios;

[TestClass]
public sealed class PerfilUsuarioTests
{
    [TestMethod]
    public void CriarDeveAssociarPerfilAoUsuario()
    {
        Guid usuarioId = Guid.NewGuid();

        PerfilUsuario perfil = PerfilUsuario.Criar(usuarioId, "  Maria Silva  ");

        Assert.AreEqual(usuarioId, perfil.UsuarioId);
        Assert.AreEqual("Maria Silva", perfil.Nome);
        Assert.AreEqual(0, perfil.Validar().Count);
    }

    [TestMethod]
    public void AtualizarNaoDevePermitirTrocaDeUsuario()
    {
        PerfilUsuario perfil = PerfilUsuario.Criar(Guid.NewGuid(), "Maria");
        PerfilUsuario perfilDeOutroUsuario = PerfilUsuario.Criar(
            Guid.NewGuid(),
            "João"
        );

        Assert.Throws<InvalidOperationException>(() =>
            perfil.Atualizar(perfilDeOutroUsuario)
        );
    }
}
