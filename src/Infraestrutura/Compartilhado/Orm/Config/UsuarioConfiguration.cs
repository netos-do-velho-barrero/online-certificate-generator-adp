using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios_dominio");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.SenhaHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DataCriacaoEmUtc).IsRequired();
        builder.Property(x => x.DataAtualizacaoEmUtc).IsRequired();
    }
}