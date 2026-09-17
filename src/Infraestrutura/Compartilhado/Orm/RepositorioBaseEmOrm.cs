using GeradorCertificadosOnline.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrm<T>(
    GeradorCertificadosOnlineDbContext contexto
) : IRepositorio<T>
    where T : EntidadeBase<T>
{
    protected GeradorCertificadosOnlineDbContext Contexto { get; } = contexto;

    protected DbSet<T> Entidades => Contexto.Set<T>();

    public async Task CadastrarAsync(
        T entidade,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entidade);

        await Entidades.AddAsync(entidade, cancellationToken);
        await PersistirAlteracoesAsync(cancellationToken);
    }

    public Task<T?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return SelecionarPorIdAsync(idSelecionado, asNoTracking: true, cancellationToken);
    }

    public async Task<T?> SelecionarPorIdAsync(
        Guid idSelecionado,
        bool asNoTracking,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<T> consulta = CriarConsulta(asNoTracking);

        return await consulta.FirstOrDefaultAsync(
            entidade => entidade.Id == idSelecionado,
            cancellationToken
        );
    }

    public Task<List<T>> SelecionarTodosAsync(
        CancellationToken cancellationToken = default
    )
    {
        return SelecionarTodosAsync(asNoTracking: true, cancellationToken);
    }

    public async Task<List<T>> SelecionarTodosAsync(
        bool asNoTracking,
        CancellationToken cancellationToken = default
    )
    {
        return await CriarConsulta(asNoTracking).ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return Entidades.AnyAsync(
            entidade => entidade.Id == idSelecionado,
            cancellationToken
        );
    }

    public async Task<bool> EditarAsync(
        Guid idSelecionado,
        T entidadeAtualizada,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entidadeAtualizada);

        T? entidade = await SelecionarPorIdAsync(
            idSelecionado,
            asNoTracking: false,
            cancellationToken
        );

        if (entidade is null)
        {
            return false;
        }

        entidade.Atualizar(entidadeAtualizada);
        await PersistirAlteracoesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        T? entidade = await SelecionarPorIdAsync(
            idSelecionado,
            asNoTracking: false,
            cancellationToken
        );

        if (entidade is null)
        {
            return false;
        }

        Entidades.Remove(entidade);
        await PersistirAlteracoesAsync(cancellationToken);
        return true;
    }

    public async Task<int> PersistirAlteracoesAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await Contexto.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException excecao)
        {
            throw new ConflitoDePersistenciaException(
                "Não foi possível persistir as alterações no banco de dados.",
                excecao
            );
        }
    }

    protected IQueryable<T> CriarConsulta(bool asNoTracking)
    {
        IQueryable<T> consulta = Entidades;

        return asNoTracking ? consulta.AsNoTracking() : consulta;
    }
}