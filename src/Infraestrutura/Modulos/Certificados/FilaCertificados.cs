using System.Threading.Channels;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class FilaCertificados : IFilaCertificados
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();

    public ValueTask<Guid> ConsumirAsync(CancellationToken ct) =>
        _channel.Reader.ReadAsync(ct);

    public Task EnfileirarAsync(Guid processamentoId, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(processamentoId, ct).AsTask();
}
