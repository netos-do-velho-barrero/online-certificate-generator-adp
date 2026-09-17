using GeradorCertificadosOnline.Aplicacao;
using GeradorCertificadosOnline.Api.Compartilhado.Logging;
using GeradorCertificadosOnline.Infraestrutura;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using FluentValidation;
using GeradorCertificadosOnline.Api.Compartilhado.Auth;
using GeradorCertificadosOnline.Api.Compartilhado.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<NewRelicOptions>(
    builder.Configuration.GetSection("NewRelic")
);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    )
);
builder.Services.AddAplicacao();
builder.Services.AddInfraestrutura(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseExceptionHandler(exceptionApp =>
    exceptionApp.Run(async context =>
    {
        Exception? excecao = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        int status = excecao switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ValidacaoDeIdentidadeException => StatusCodes.Status400BadRequest,
            ConflitoDeIdentidadeException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        ProblemDetails problema = new()
        {
            Status = status,
            Title = status switch
            {
                400 => "Erro de validação",
                404 => "Recurso não encontrado",
                409 => "Conflito",
                401 => "Não autenticado",
                _ => "Erro interno"
            },
            Detail = status == 500
                ? "Ocorreu um erro interno."
                : excecao?.Message,
            Type = $"https://httpstatuses.com/{status}"
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problema);
    })
);
app.UseLoggingCompartilhado();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
