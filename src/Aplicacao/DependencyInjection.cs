using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;

namespace GeradorCertificadosOnline.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicacao(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        }
        );
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
