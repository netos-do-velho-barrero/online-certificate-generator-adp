using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public sealed class ProcessamentoCertificadosConfiguration
    : IEntityTypeConfiguration<ProcessamentoCertificados>
{
    public void Configure(EntityTypeBuilder<ProcessamentoCertificados> builder)
    {
        builder.ToTable("processamentos_certificados");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status)
            .HasConversion(
                status => status.ToString(),
                status => Enum.Parse<StatusProcessamento>(status)
            )
            .HasColumnType("text")
            .IsRequired();
        builder.Property(x => x.CaminhoZip).HasMaxLength(500);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.HasIndex(x => new { x.CursoId, x.UsuarioId })
            .HasDatabaseName("ix_processamentos_curso_usuario");
        builder.HasIndex(x => new { x.CursoId, x.UsuarioId })
            .HasDatabaseName("ux_processamentos_ativos")
            .IsUnique()
            .HasFilter("\"Status\" NOT IN ('Concluido', 'Falha')");
        builder.HasMany(x => x.Certificados)
            .WithOne()
            .HasForeignKey(x => x.ProcessamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.ToTable("certificados");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NomeAluno).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status)
            .HasConversion(
                status => status.ToString(),
                status => Enum.Parse<StatusCertificado>(status)
            )
            .HasColumnType("text")
            .IsRequired();
        builder.Property(x => x.CaminhoPdf).HasMaxLength(500);
        builder.Property(x => x.Erro).HasMaxLength(2000);
        builder.HasIndex(x => new { x.ProcessamentoId, x.UsuarioId });
        builder.HasIndex(x => new { x.CursoId, x.UsuarioId });
    }
}