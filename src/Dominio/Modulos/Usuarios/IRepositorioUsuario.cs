using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Usuarios;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    Task<Usuario?> SelecionarPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );
}