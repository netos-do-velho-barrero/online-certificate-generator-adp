using System.Text;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        JwtOptions options = configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("A seção Jwt não foi configurada.");
        if (string.IsNullOrWhiteSpace(options.Secret) || options.Secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret deve possuir pelo menos 32 caracteres.");
        }

        services.AddHttpContextAccessor();
        services.AddScoped<IProvedorDeUsuario, UserProvider>();
        services.AddSingleton<IEmissorDeTokens, JwtProvider>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(options.Secret)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
        services.AddAuthorization();
        return services;
    }
}