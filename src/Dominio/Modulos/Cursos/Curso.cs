using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Cursos;

public sealed class Curso : EntidadeBase<Curso>
{
    private Curso()
    {
    }

    private Curso(
        Guid id,
        string nome,
        string? descricao,
        int cargaHoraria,
        DateOnly dataConclusao
    )
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public int CargaHoraria { get; private set; }
    public DateOnly DataConclusao { get; private set; }

    public static Curso Criar(
        string nome,
        string? descricao,
        int cargaHoraria,
        DateOnly dataConclusao,
        Guid? id = null
    )
    {
        string nomeNormalizado = NormalizarNome(nome);
        string? descricaoNormalizada = NormalizarDescricao(descricao);
        ValidarCargaHoraria(cargaHoraria);

        return new Curso(
            id ?? Guid.CreateVersion7(),
            nomeNormalizado,
            descricaoNormalizada,
            cargaHoraria,
            dataConclusao
        );
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
        {
            erros.Add(new ErroValidacao(nameof(Nome), "O nome é obrigatório."));
        }
        else if (Nome.Length > 200)
        {
            erros.Add(new ErroValidacao(nameof(Nome), "O nome deve ter no máximo 200 caracteres."));
        }

        if (Descricao is not null && Descricao.Length > 500)
        {
            erros.Add(new ErroValidacao(nameof(Descricao), "A descrição deve ter no máximo 500 caracteres."));
        }

        if (CargaHoraria <= 0)
        {
            erros.Add(new ErroValidacao(nameof(CargaHoraria), "A carga horária deve ser maior que zero."));
        }

        return erros;
    }

    public override void Atualizar(Curso entidadeAtualizada)
    {
        ArgumentNullException.ThrowIfNull(entidadeAtualizada);

        Nome = NormalizarNome(entidadeAtualizada.Nome);
        Descricao = NormalizarDescricao(entidadeAtualizada.Descricao);
        ValidarCargaHoraria(entidadeAtualizada.CargaHoraria);
        CargaHoraria = entidadeAtualizada.CargaHoraria;
        DataConclusao = entidadeAtualizada.DataConclusao;
    }

    private static string NormalizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));
        }

        string nomeNormalizado = nome.Trim();
        if (nomeNormalizado.Length > 200)
        {
            throw new ArgumentException(
                "O nome deve ter no máximo 200 caracteres.",
                nameof(nome)
            );
        }

        return nomeNormalizado;
    }

    private static string? NormalizarDescricao(string? descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return null;
        }

        string descricaoNormalizada = descricao.Trim();
        if (descricaoNormalizada.Length > 500)
        {
            throw new ArgumentException(
                "A descrição deve ter no máximo 500 caracteres.",
                nameof(descricao)
            );
        }

        return descricaoNormalizada;
    }

    private static void ValidarCargaHoraria(int cargaHoraria)
    {
        if (cargaHoraria <= 0)
        {
            throw new ArgumentException(
                "A carga horária deve ser maior que zero.",
                nameof(cargaHoraria)
            );
        }
    }
}
