using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeradorCertificadosOnline.IntegrationTests.Compartilhado.Orm;

[TestClass]
public sealed class RepositorioBaseEmOrmIntegrationTests
{
    private SqliteConnection conexao = null!;
    private GeradorCertificadosOnlineDbContext contexto = null!;
    private RepositorioTeste repositorio = null!;

    [TestInitialize]
    public async Task InicializarAsync()
    {
        conexao = new SqliteConnection("DataSource=:memory:");
        await conexao.OpenAsync();

        var opcoes = new DbContextOptionsBuilder<GeradorCertificadosOnlineDbContext>()
            .UseSqlite(conexao)
            .Options;

        contexto = new ContextoDeTeste(opcoes);
        await contexto.Database.EnsureCreatedAsync();
        repositorio = new RepositorioTeste(contexto);
    }

    [TestCleanup]
    public async Task FinalizarAsync()
    {
        await contexto.DisposeAsync();
        await conexao.DisposeAsync();
    }

    [TestMethod]
    public async Task DeveCadastrarBuscarVerificarEditarEExcluir()
    {
        EntidadeTeste entidade = new("original");

        await repositorio.CadastrarAsync(entidade);

        Assert.IsTrue(await repositorio.ExisteAsync(entidade.Id));
        EntidadeTeste? encontrada = await repositorio.SelecionarPorIdAsync(entidade.Id);
        Assert.IsNotNull(encontrada);
        Assert.AreEqual("original", encontrada.Nome);

        bool editada = await repositorio.EditarAsync(
            entidade.Id,
            new EntidadeTeste("atualizada")
        );

        Assert.IsTrue(editada);
        Assert.AreEqual(
            "atualizada",
            (await repositorio.SelecionarPorIdAsync(entidade.Id))!.Nome
        );

        bool excluida = await repositorio.ExcluirAsync(entidade.Id);

        Assert.IsTrue(excluida);
        Assert.IsFalse(await repositorio.ExisteAsync(entidade.Id));
    }

    private sealed class RepositorioTeste(GeradorCertificadosOnlineDbContext contexto)
        : RepositorioBaseEmOrm<EntidadeTeste>(contexto);

    private sealed class ContextoDeTeste(
        DbContextOptions<GeradorCertificadosOnlineDbContext> opcoes
    ) : GeradorCertificadosOnlineDbContext(opcoes)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EntidadeTeste>().HasKey(entidade => entidade.Id);
        }
    }

    private sealed class EntidadeTeste(string nome) : EntidadeBase<EntidadeTeste>
    {
        public string Nome { get; private set; } = nome;

        public override IReadOnlyList<ErroValidacao> Validar() => [];

        public override void Atualizar(EntidadeTeste entidadeAtualizada)
        {
            Nome = entidadeAtualizada.Nome;
        }
    }
}
