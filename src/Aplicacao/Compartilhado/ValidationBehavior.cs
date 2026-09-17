using FluentValidation;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Compartilhado;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validadores
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!validadores.Any())
        {
            return await next();
        }

        var contexto = new ValidationContext<TRequest>(request);
        var resultados = await Task.WhenAll(
            validadores.Select(validador => validador.ValidateAsync(contexto, cancellationToken))
        );
        var falhas = resultados
            .SelectMany(resultado => resultado.Errors)
            .Where(falha => falha is not null)
            .ToList();

        if (falhas.Count != 0)
        {
            throw new ValidationException(falhas);
        }

        return await next();
    }
}
