using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Usuarios;

public sealed class RepositorioUsuarioEmOrm(
    GeradorCertificadosOnlineDbContext contexto
) : RepositorioBaseEmOrm<Usuario>(contexto), IRepositorioUsuario
{
    public Task<Usuario?> SelecionarPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        string emailNormalizado = email.Trim().ToLowerInvariant();
        return Entidades.AsNoTracking().FirstOrDefaultAsync(
            usuario => usuario.Email == emailNormalizado,
            cancellationToken
        );
    }
}