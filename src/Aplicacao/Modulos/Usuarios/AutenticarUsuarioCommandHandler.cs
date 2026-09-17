using FluentValidation;
using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;
using IdentityUsuarioDto = GeradorCertificadosOnline.Dominio.Compartilhado.Auth.UsuarioDto;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;

public sealed record AutenticarUsuarioCommand(string Email, string Senha)
    : IRequest<TokenDto?>;

public sealed class AutenticarUsuarioCommandValidator
    : AbstractValidator<AutenticarUsuarioCommand>
{
    public AutenticarUsuarioCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Senha).NotEmpty();
    }
}

public sealed class AutenticarUsuarioCommandHandler(
    IGerenciadorDeIdentidade identidade,
    IEmissorDeTokens emissorDeTokens
) : IRequestHandler<AutenticarUsuarioCommand, TokenDto?>
{
    public async Task<TokenDto?> Handle(
        AutenticarUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        IdentityUsuarioDto? usuario = await identidade.ChecarValidadeDeSenhaAsync(
            request.Email,
            request.Senha,
            TipoUsuario.Cliente
        );

        if (usuario is null)
        {
            return null;
        }

        AccessToken token = emissorDeTokens.CriarToken(
            usuario.Id,
            usuario.Email,
            TipoUsuario.Cliente
        );
        return new TokenDto(token.Token, token.DataExpiracaoEmUtc);
    }
}