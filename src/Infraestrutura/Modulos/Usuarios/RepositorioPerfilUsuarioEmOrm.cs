using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Usuarios;

public sealed class RepositorioPerfilUsuarioEmOrm(
    GeradorCertificadosOnlineDbContext contexto
) : RepositorioBaseEmOrm<PerfilUsuario>(contexto), IRepositorioPerfilUsuario
{
    public Task<PerfilUsuario?> SelecionarPorUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default
    )
    {
        return Entidades.AsNoTracking().FirstOrDefaultAsync(
            perfil => perfil.UsuarioId == usuarioId,
            cancellationToken
        );
    }
}