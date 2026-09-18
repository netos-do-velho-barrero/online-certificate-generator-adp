using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;
public interface IRepositorioProcessamento : IRepositorio<ProcessamentoCertificados>
{
    Task<bool> PossuiAtivoAsync(Guid cursoId, Guid usuarioId, CancellationToken ct = default);
    Task<ProcessamentoCertificados?> ObterComCertificadosAsync(Guid cursoId, Guid usuarioId, CancellationToken ct = default);
    Task<ProcessamentoCertificados?> ObterPorIdComCertificadosAsync(Guid processamentoId, Guid? usuarioId, bool rastrear, CancellationToken ct = default);
    Task PersistirAsync(CancellationToken ct = default);
}
public interface IFilaCertificados { Task EnfileirarAsync(Guid processamentoId, CancellationToken ct = default); }
public interface IProcessadorCertificados { Task ProcessarAsync(Guid processamentoId, CancellationToken ct = default); }
public interface IArmazenamentoArquivos
{
    Task<string> SalvarAsync(string pasta, string nomeArquivo, byte[] conteudo, CancellationToken ct = default);
    Task<byte[]> LerAsync(string caminho, CancellationToken ct = default);
}
public interface IGeradorPdfCertificado
{
    byte[] Gerar(string nomeAluno, string nomeCurso, int cargaHoraria, DateOnly dataEmissao);
}
public interface IGeradorZip
{
    byte[] Gerar(IReadOnlyList<(string NomeArquivo, byte[] Conteudo)> arquivos);
}
