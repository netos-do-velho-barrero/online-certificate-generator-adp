using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;

namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

public enum StatusProcessamento { Pendente, GerandoCertificados, GerandoZip, Concluido, Falha }
public enum StatusCertificado { Pendente, Gerando, Gerado, Falha }

public sealed class ProcessamentoCertificados : EntidadeBase<ProcessamentoCertificados>, IEntidadeDeUsuario
{
    private ProcessamentoCertificados() { }
    private ProcessamentoCertificados(Guid id, Guid cursoId, Guid usuarioId) { Id = id; CursoId = cursoId; UsuarioId = usuarioId; }
    public Guid CursoId { get; private set; }
    public Guid UsuarioId { get; set; }
    public StatusProcessamento Status { get; private set; } = StatusProcessamento.Pendente;
    public string? CaminhoZip { get; private set; }
    public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;
    public DateTime? ConcluidoEm { get; private set; }
    public ICollection<Certificado> Certificados { get; private set; } = new List<Certificado>();
    public static ProcessamentoCertificados Criar(Guid cursoId, Guid usuarioId, Guid? id = null) =>
        new(id ?? Guid.CreateVersion7(), cursoId, usuarioId);
    public bool Finalizado => Status is StatusProcessamento.Concluido or StatusProcessamento.Falha;
    public void IniciarCertificados()
    {
        if (Finalizado) return;
        Status = StatusProcessamento.GerandoCertificados;
    }
    public void IniciarZip()
    {
        if (Finalizado) return;
        Status = StatusProcessamento.GerandoZip;
    }
    public void Concluir(string caminho)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminho);
        Status = StatusProcessamento.Concluido;
        CaminhoZip = caminho;
        ConcluidoEm = DateTime.UtcNow;
    }
    public void Falhar()
    {
        if (Status == StatusProcessamento.Concluido) return;
        Status = StatusProcessamento.Falha;
        ConcluidoEm = DateTime.UtcNow;
    }
    public override IReadOnlyList<ErroValidacao> Validar() => [];
    public override void Atualizar(ProcessamentoCertificados entidadeAtualizada) { Status = entidadeAtualizada.Status; CaminhoZip = entidadeAtualizada.CaminhoZip; ConcluidoEm = entidadeAtualizada.ConcluidoEm; }
}

public sealed class Certificado : EntidadeBase<Certificado>, IEntidadeDeUsuario
{
    private Certificado() { }
    private Certificado(Guid id, Guid processamentoId, Guid cursoId, Guid usuarioId, string nomeAluno)
    { Id = id; ProcessamentoId = processamentoId; CursoId = cursoId; UsuarioId = usuarioId; NomeAluno = nomeAluno; }
    public Guid ProcessamentoId { get; private set; }
    public Guid CursoId { get; private set; }
    public Guid UsuarioId { get; set; }
    public string NomeAluno { get; private set; } = string.Empty;
    public StatusCertificado Status { get; private set; } = StatusCertificado.Pendente;
    public string? CaminhoPdf { get; private set; }
    public DateTime? GeradoEm { get; private set; }
    public string? Erro { get; private set; }
    public static Certificado Criar(Guid processamentoId, Guid cursoId, Guid usuarioId, string nomeAluno, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(nomeAluno) || nomeAluno.Trim().Length > 200) throw new ArgumentException("Nome do aluno inválido.", nameof(nomeAluno));
        return new(id ?? Guid.CreateVersion7(), processamentoId, cursoId, usuarioId, nomeAluno.Trim());
    }
    public void Iniciar()
    {
        if (Status is StatusCertificado.Gerado or StatusCertificado.Falha) return;
        Status = StatusCertificado.Gerando;
        Erro = null;
    }
    public void Gerar(string caminho)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminho);
        Status = StatusCertificado.Gerado;
        CaminhoPdf = caminho;
        GeradoEm = DateTime.UtcNow;
        Erro = null;
    }
    public void Falhar(string erro)
    {
        Erro = string.IsNullOrWhiteSpace(erro) ? "Falha não identificada." : erro;
        Status = StatusCertificado.Falha;
    }
    public override IReadOnlyList<ErroValidacao> Validar() => [];
    public override void Atualizar(Certificado entidadeAtualizada) { Status = entidadeAtualizada.Status; CaminhoPdf = entidadeAtualizada.CaminhoPdf; GeradoEm = entidadeAtualizada.GeradoEm; Erro = entidadeAtualizada.Erro; }
}
