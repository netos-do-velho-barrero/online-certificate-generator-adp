using System.Diagnostics;

namespace GeradorCertificadosOnline.Api.Compartilhado.Logging;

public static class LoggingExtensions
{
    public static IApplicationBuilder UseLoggingCompartilhado(
        this IApplicationBuilder app
    )
    {
        return app.Use(async (context, next) =>
        {
            string correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? Guid.NewGuid().ToString("N");
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            context.Items["CorrelationId"] = correlationId;

            ILogger logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Request");
            Stopwatch cronometro = Stopwatch.StartNew();

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Method"] = context.Request.Method,
                ["Path"] = context.Request.Path.ToString()
            }))
            {
                try
                {
                    await next();
                    logger.LogInformation(
                        "Requisição processada com status {StatusCode} em {ElapsedMilliseconds}ms",
                        context.Response.StatusCode,
                        cronometro.ElapsedMilliseconds
                    );
                }
                catch (Exception excecao)
                {
                    logger.LogError(
                        excecao,
                        "Erro não tratado ao processar requisição em {ElapsedMilliseconds}ms",
                        cronometro.ElapsedMilliseconds
                    );
                    throw;
                }
            }
        });
    }
}