using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

public sealed class ProcessadorCertificadosBackgroundService(
    FilaCertificados fila,
    IServiceScopeFactory scopeFactory,
    ILogger<ProcessadorCertificadosBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (Guid processamentoId in fila.ConsumirTodosAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                IProcessadorCertificados processador =
                    scope.ServiceProvider.GetRequiredService<IProcessadorCertificados>();
                await processador.ProcessarAsync(processamentoId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception excecao)
            {
                logger.LogError(
                    excecao,
                    "Falha no processamento assíncrono {ProcessamentoId}.",
                    processamentoId
                );
                using IServiceScope scope = scopeFactory.CreateScope();
                IRepositorioProcessamento repositorio =
                    scope.ServiceProvider.GetRequiredService<IRepositorioProcessamento>();
                ProcessamentoCertificados? processamento =
                    await repositorio.ObterPorIdComCertificadosAsync(
                        processamentoId,
                        null,
                        true,
                        CancellationToken.None
                    );
                if (processamento is not null)
                {
                    processamento.Falhar();
                    await repositorio.PersistirAsync(CancellationToken.None);
                }
            }
        }
    }
}

internal static class FilaCertificadosExtensions
{
    public static async IAsyncEnumerable<Guid> ConsumirTodosAsync(
        this FilaCertificados fila,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct
    )
    {
        while (!ct.IsCancellationRequested)
        {
            yield return await fila.ConsumirAsync(ct);
        }
    }
}
