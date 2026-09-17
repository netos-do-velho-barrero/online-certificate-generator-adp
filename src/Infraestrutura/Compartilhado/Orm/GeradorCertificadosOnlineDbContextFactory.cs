using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;

public sealed class GeradorCertificadosOnlineDbContextFactory
    : IDesignTimeDbContextFactory<GeradorCertificadosOnlineDbContext>
{
    public GeradorCertificadosOnlineDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<GeradorCertificadosOnlineDbContext> builder = new();
        builder.UseNpgsql(
            "Host=localhost;Port=5432;Database=gerador_certificados;Username=postgres;Password=postgres"
        );
        return new GeradorCertificadosOnlineDbContext(builder.Options);
    }
}
