using GeradorCertificadosOnline.Aplicacao;
using GeradorCertificadosOnline.Api.Compartilhado.Logging;
using GeradorCertificadosOnline.Infraestrutura;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using FluentValidation;
using GeradorCertificadosOnline.Api.Compartilhado.Auth;
using Microsoft.OpenApi.Models;
using GeradorCertificadosOnline.Dominio.Compartilhado;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gerador de Certificados Online API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe somente o JWT. O Swagger adicionará o prefixo Bearer."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }] = []
    });
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<NewRelicOptions>(
    builder.Configuration.GetSection("NewRelic")
);
builder.Services.AddJwtAuthentication(builder.Configuration);
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

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(exceptionApp =>
    exceptionApp.Run(async context =>
    {
        Exception? excecao = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        int status = excecao switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ValidacaoDeIdentidadeException => StatusCodes.Status400BadRequest,
            ConflitoDeIdentidadeException => StatusCodes.Status409Conflict,
            ConflitoDeRegraDeNegocioException => StatusCodes.Status409Conflict,
            ConflitoDePersistenciaException => StatusCodes.Status409Conflict,
            InvalidOperationException => StatusCodes.Status409Conflict,
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
