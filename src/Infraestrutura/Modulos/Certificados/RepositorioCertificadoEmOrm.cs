using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioProcessamentoEmOrm(
    GeradorCertificadosOnlineDbContext contexto
) : RepositorioBaseEmOrm<ProcessamentoCertificados>(contexto), IRepositorioProcessamento
{
    public Task<bool> PossuiAtivoAsync(
        Guid cursoId,
        Guid usuarioId,
        CancellationToken ct = default
    ) =>
        Entidades.AnyAsync(
            x => x.CursoId == cursoId &&
                 x.UsuarioId == usuarioId &&
                 x.Status != StatusProcessamento.Concluido &&
                 x.Status != StatusProcessamento.Falha,
            ct
        );

    public Task<ProcessamentoCertificados?> ObterComCertificadosAsync(
        Guid cursoId,
        Guid usuarioId,
        CancellationToken ct = default
    ) =>
        Entidades
            .Include(x => x.Certificados)
            .Where(x => x.CursoId == cursoId && x.UsuarioId == usuarioId)
            .OrderByDescending(x => x.CriadoEm)
            .FirstOrDefaultAsync(ct);

    public Task<ProcessamentoCertificados?> ObterPorIdComCertificadosAsync(
        Guid processamentoId,
        Guid? usuarioId,
        bool rastrear,
        CancellationToken ct = default
    )
    {
        IQueryable<ProcessamentoCertificados> consulta = Entidades
            .Include(x => x.Certificados)
            .Where(x => x.Id == processamentoId &&
                (!usuarioId.HasValue || x.UsuarioId == usuarioId.Value));

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return consulta.FirstOrDefaultAsync(ct);
    }

    public Task PersistirAsync(CancellationToken ct = default) =>
        PersistirAlteracoesAsync(ct);
}