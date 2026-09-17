using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;

namespace GeradorCertificadosOnline.Dominio.Modulos.Usuarios;

public sealed class PerfilUsuario : EntidadeBase<PerfilUsuario>, IEntidadeDeUsuario
{
    private PerfilUsuario()
    {
    }

    private PerfilUsuario(
        Guid id,
        Guid usuarioId,
        string nome,
        DateTime agoraUtc
    )
    {
        Id = id;
        UsuarioId = usuarioId;
        Nome = nome;
        DataCriacaoEmUtc = agoraUtc;
        DataAtualizacaoEmUtc = agoraUtc;
    }

    public Guid UsuarioId { get; set; }
    public string Nome { get; private set; } = string.Empty;
    public DateTime DataCriacaoEmUtc { get; private set; }
    public DateTime DataAtualizacaoEmUtc { get; private set; }

    public static PerfilUsuario Criar(
        Guid usuarioId,
        string nome,
        Guid? id = null,
        DateTime? agoraUtc = null
    )
    {
        ValidarUsuarioId(usuarioId);
        string nomeNormalizado = NormalizarNome(nome);
        DateTime data = NormalizarDataUtc(agoraUtc ?? DateTime.UtcNow);

        return new PerfilUsuario(
            id ?? Guid.CreateVersion7(),
            usuarioId,
            nomeNormalizado,
            data
        );
    }

    public void AlterarNome(string nome, DateTime? agoraUtc = null)
    {
        Nome = NormalizarNome(nome);
        DataAtualizacaoEmUtc = NormalizarDataUtc(agoraUtc ?? DateTime.UtcNow);
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (UsuarioId == Guid.Empty)
        {
            erros.Add(new ErroValidacao(nameof(UsuarioId), "O usuário é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(Nome))
        {
            erros.Add(new ErroValidacao(nameof(Nome), "O nome é obrigatório."));
        }

        return erros;
    }

    public override void Atualizar(PerfilUsuario entidadeAtualizada)
    {
        ArgumentNullException.ThrowIfNull(entidadeAtualizada);

        if (entidadeAtualizada.UsuarioId != UsuarioId)
        {
            throw new InvalidOperationException(
                "Não é permitido alterar o usuário proprietário do perfil."
            );
        }

        AlterarNome(entidadeAtualizada.Nome);
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

    private static void ValidarUsuarioId(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(usuarioId));
        }
    }

    private static DateTime NormalizarDataUtc(DateTime data)
    {
        return data.Kind == DateTimeKind.Utc ? data : data.ToUniversalTime();
    }
}