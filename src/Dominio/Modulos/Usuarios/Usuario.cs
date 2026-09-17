using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Usuarios;

public sealed class Usuario : EntidadeBase<Usuario>
{
    private Usuario()
    {
    }

    private Usuario(Guid id, string email, string senhaHash, DateTime agoraUtc)
    {
        Id = id;
        Email = email;
        SenhaHash = senhaHash;
        DataCriacaoEmUtc = agoraUtc;
        DataAtualizacaoEmUtc = agoraUtc;
    }

    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCriacaoEmUtc { get; private set; }
    public DateTime DataAtualizacaoEmUtc { get; private set; }

    public static Usuario Criar(
        string email,
        string senhaHash,
        Guid? id = null,
        DateTime? agoraUtc = null
    )
    {
        string emailNormalizado = NormalizarEmail(email);
        ValidarSenhaHash(senhaHash);

        DateTime data = NormalizarDataUtc(agoraUtc ?? DateTime.UtcNow);
        return new Usuario(id ?? Guid.CreateVersion7(), emailNormalizado, senhaHash, data);
    }

    public void AlterarEmail(string email, DateTime? agoraUtc = null)
    {
        Email = NormalizarEmail(email);
        MarcarAtualizacao(agoraUtc);
    }

    public void AlterarSenhaHash(string senhaHash, DateTime? agoraUtc = null)
    {
        ValidarSenhaHash(senhaHash);
        SenhaHash = senhaHash;
        MarcarAtualizacao(agoraUtc);
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Email))
        {
            erros.Add(new ErroValidacao(nameof(Email), "O e-mail é obrigatório."));
        }
        else if (!Email.Contains('@', StringComparison.Ordinal))
        {
            erros.Add(new ErroValidacao(nameof(Email), "O e-mail informado é inválido."));
        }

        if (string.IsNullOrWhiteSpace(SenhaHash))
        {
            erros.Add(new ErroValidacao(nameof(SenhaHash), "O hash da senha é obrigatório."));
        }

        return erros;
    }

    public override void Atualizar(Usuario entidadeAtualizada)
    {
        ArgumentNullException.ThrowIfNull(entidadeAtualizada);
        AlterarEmail(entidadeAtualizada.Email);
        AlterarSenhaHash(entidadeAtualizada.SenhaHash);
    }

    private static string NormalizarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail é obrigatório.", nameof(email));
        }

        string emailNormalizado = email.Trim().ToLowerInvariant();
        if (emailNormalizado.Length > 320 || !emailNormalizado.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("O e-mail informado é inválido.", nameof(email));
        }

        return emailNormalizado;
    }

    private static void ValidarSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new ArgumentException("O hash da senha é obrigatório.", nameof(senhaHash));
        }
    }

    private void MarcarAtualizacao(DateTime? agoraUtc)
    {
        DataAtualizacaoEmUtc = NormalizarDataUtc(agoraUtc ?? DateTime.UtcNow);
    }

    private static DateTime NormalizarDataUtc(DateTime data)
    {
        return data.Kind == DateTimeKind.Utc ? data : data.ToUniversalTime();
    }
}