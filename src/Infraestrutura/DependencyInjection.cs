using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Auth;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using GeradorCertificadosOnline.Infraestrutura.Modulos.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorCertificadosOnline.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestrutura(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            configuration.GetConnectionString("PostgresEF")
            ?? throw new InvalidOperationException(
                "A connection string 'PostgresEF' não foi configurada."
            );

        services.AddDbContext<GeradorCertificadosOnlineDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<GeradorCertificadosOnlineDbContext>();

        services.AddScoped<IGerenciadorDeIdentidade, GerenciadorDeIdentidade>();
        services.AddScoped<IRepositorioUsuario, RepositorioUsuarioEmOrm>();
        services.AddScoped<IRepositorioPerfilUsuario, RepositorioPerfilUsuarioEmOrm>();

        return services;
    }
}
