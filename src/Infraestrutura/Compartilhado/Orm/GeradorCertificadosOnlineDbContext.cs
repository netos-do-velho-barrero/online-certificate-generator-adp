using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;

public class GeradorCertificadosOnlineDbContext(
    DbContextOptions<GeradorCertificadosOnlineDbContext> options,
    IProvedorDeUsuario? provedorDeUsuario = null
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    private Guid UsuarioAtualId => provedorDeUsuario?.Id ?? Guid.Empty;
    private bool PossuiUsuarioAtual => provedorDeUsuario is not null;

    private static readonly Guid TipoUsuarioClienteId =
        new("01a058f4-a048-79a3-b1a6-0f01d629a126");
    private static readonly Guid TipoUsuarioEstabelecimentoId =
        new("01a06851-5e71-7ae2-822d-21e2fadcffa4");

    public DbSet<Usuario> UsuariosDominio => Set<Usuario>();
    public DbSet<PerfilUsuario> PerfisUsuarios => Set<PerfilUsuario>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<ProcessamentoCertificados> ProcessamentosCertificados => Set<ProcessamentoCertificados>();
    public DbSet<Certificado> Certificados => Set<Certificado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfiguracaoOrmGeral.Aplicar(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorCertificadosOnlineDbContext).Assembly);

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = TipoUsuarioClienteId,
                Name = TipoUsuario.Cliente.ToString(),
                NormalizedName = TipoUsuario.Cliente.ToString().ToUpperInvariant(),
                ConcurrencyStamp = "01a058f7-9492-73bc-8e4b-934c53594ed6"
            },
            new IdentityRole<Guid>
            {
                Id = TipoUsuarioEstabelecimentoId,
                Name = TipoUsuario.Estabelecimento.ToString(),
                NormalizedName = TipoUsuario.Estabelecimento.ToString().ToUpperInvariant(),
                ConcurrencyStamp = "01a06852-c767-7d97-84e4-6b5f0775f3e5"
            }
        );

        // Isola leituras por usuário autenticado. Contextos técnicos sem provedor
        // continuam disponíveis para migrations e tarefas administrativas.
        modelBuilder.Entity<PerfilUsuario>().HasQueryFilter(perfil =>
            !PossuiUsuarioAtual || perfil.UsuarioId == UsuarioAtualId
        );
    }

    public override int SaveChanges()
    {
        AplicarRegrasDePropriedade();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        AplicarRegrasDePropriedade();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AplicarRegrasDePropriedade()
    {
        if (provedorDeUsuario is null || !provedorDeUsuario.EstaAutenticado)
        {
            return;
        }

        if (provedorDeUsuario.Id is not Guid usuarioAtualId)
        {
            throw new UnauthorizedAccessException(
                "Não é possível salvar entidades do usuário sem estar autenticado."
            );
        }

        foreach (var entry in ChangeTracker.Entries<IEntidadeDeUsuario>())
        {
            PropertyEntry propriedadeUsuario = entry.Property(
                nameof(IEntidadeDeUsuario.UsuarioId)
            );

            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.UsuarioId == Guid.Empty)
                    {
                        propriedadeUsuario.CurrentValue = usuarioAtualId;
                    }
                    else if (entry.Entity.UsuarioId != usuarioAtualId)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de criar entidade para outro usuário."
                        );
                    }

                    break;

                case EntityState.Modified:
                    Guid usuarioOriginalId = (Guid)propriedadeUsuario.OriginalValue!;

                    if (usuarioOriginalId != usuarioAtualId)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de modificar entidade de outro usuário."
                        );
                    }

                    if (entry.Entity.UsuarioId != usuarioOriginalId)
                    {
                        throw new UnauthorizedAccessException(
                            "Não é permitido alterar o usuário de uma entidade."
                        );
                    }

                    break;

                case EntityState.Deleted:
                    if ((Guid)propriedadeUsuario.OriginalValue! != usuarioAtualId)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de excluir entidade de outro usuário."
                        );
                    }

                    break;
            }
        }
    }
}