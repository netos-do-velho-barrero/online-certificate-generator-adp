using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public sealed class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
{
    public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
    {
        builder.ToTable("perfis_usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.UsuarioId).IsUnique();
        builder.Property(x => x.DataCriacaoEmUtc).IsRequired();
        builder.Property(x => x.DataAtualizacaoEmUtc).IsRequired();
    }
}