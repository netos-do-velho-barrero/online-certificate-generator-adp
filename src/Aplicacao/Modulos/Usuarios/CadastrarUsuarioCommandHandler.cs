using FluentValidation;
using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using MediatR;
using IdentityUsuarioDto = GeradorCertificadosOnline.Dominio.Compartilhado.Auth.UsuarioDto;
using UsuarioAplicacaoDto = GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs.UsuarioDto;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;

public sealed record CadastrarUsuarioCommand(string Email, string Senha)
    : IRequest<UsuarioAplicacaoDto>;

public sealed class CadastrarUsuarioCommandValidator
    : AbstractValidator<CadastrarUsuarioCommand>
{
    public CadastrarUsuarioCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Senha)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
    }
}

public sealed class CadastrarUsuarioCommandHandler(
    IGerenciadorDeIdentidade identidade,
    IRepositorioUsuario repositorioUsuario
) : IRequestHandler<CadastrarUsuarioCommand, UsuarioAplicacaoDto>
{
    public async Task<UsuarioAplicacaoDto> Handle(
        CadastrarUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid id = Guid.CreateVersion7();
        IdentityUsuarioDto usuarioIdentity = await identidade.CadastrarAsync(
            id,
            request.Email,
            request.Senha,
            TipoUsuario.Cliente
        );

        try
        {
            Usuario usuario = Usuario.Criar(
                usuarioIdentity.Email,
                identidade.CriarHashDeSenha(id, request.Senha),
                id
            );
            await repositorioUsuario.CadastrarAsync(usuario, cancellationToken);
            return new UsuarioAplicacaoDto(usuario.Id, usuario.Email);
        }
        catch
        {
            await identidade.ExcluirAsync(id);
            throw;
        }
    }
}