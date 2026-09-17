using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Usuarios;

public interface IRepositorioPerfilUsuario : IRepositorio<PerfilUsuario>
{
    Task<PerfilUsuario?> SelecionarPorUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default
    );
}