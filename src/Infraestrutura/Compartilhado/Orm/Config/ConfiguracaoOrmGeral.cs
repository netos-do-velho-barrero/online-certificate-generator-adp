using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public static class ConfiguracaoOrmGeral
{
    public static void Aplicar(ModelBuilder modelBuilder)
    {
        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            // Chaves Guid são geradas pela aplicação para manter IDs ordenáveis.
            var chave = entidade.FindPrimaryKey();
            if (chave is not null && chave.Properties.Count == 1)
            {
                chave.Properties[0].ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
            }

            foreach (var propriedade in entidade.GetProperties())
            {
                if (propriedade.ClrType == typeof(string))
                {
                    propriedade.SetMaxLength(500);
                }

                if (propriedade.ClrType.IsEnum)
                {
                    propriedade.SetColumnType("integer");
                }

                if (propriedade.ClrType == typeof(DateTime)
                    || propriedade.ClrType == typeof(DateTime?))
                {
                    propriedade.SetColumnType("timestamp with time zone");
                }
            }
        }

        // As configurações de entidades e relacionamentos ficam nos módulos.
        // Não há regras específicas de Usuário, Curso ou Certificado nesta base.
        modelBuilder.Entity<IdentityUser<Guid>>().ToTable("usuarios");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("papeis");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("usuarios_papeis");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("usuarios_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("usuarios_logins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("papeis_claims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("usuarios_tokens");
    }
}
