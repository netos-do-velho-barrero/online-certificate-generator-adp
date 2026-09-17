using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

public sealed class JwtProvider(IOptions<JwtOptions> options) : IEmissorDeTokens
{
    private readonly JwtOptions configuracao = options.Value;

    public AccessToken CriarToken(Guid usuarioId, string email, TipoUsuario tipoUsuario)
    {
        if (string.IsNullOrWhiteSpace(configuracao.Secret) || configuracao.Secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret deve possuir pelo menos 32 caracteres.");
        }

        DateTime expiracao = DateTime.UtcNow.AddMinutes(configuracao.AccessTokenMinutes);
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, tipoUsuario.ToString())
        ];
        SigningCredentials credenciais = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracao.Secret)),
            SecurityAlgorithms.HmacSha256
        );
        JwtSecurityToken token = new(
            configuracao.Issuer,
            configuracao.Audience,
            claims,
            expires: expiracao,
            signingCredentials: credenciais
        );
        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiracao);
    }
}