using FluentValidation;
using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;

public sealed record CadastrarPerfilUsuarioCommand(string Nome)
    : IRequest<PerfilUsuarioDto>;

public sealed class CadastrarPerfilUsuarioCommandValidator
    : AbstractValidator<CadastrarPerfilUsuarioCommand>
{
    public CadastrarPerfilUsuarioCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
    }
}

public sealed class CadastrarPerfilUsuarioCommandHandler(
    IProvedorDeUsuario provedorDeUsuario,
    IRepositorioPerfilUsuario repositorioPerfil
) : IRequestHandler<CadastrarPerfilUsuarioCommand, PerfilUsuarioDto>
{
    public async Task<PerfilUsuarioDto> Handle(
        CadastrarPerfilUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid usuarioId = ObterUsuarioId();
        if (await repositorioPerfil.SelecionarPorUsuarioIdAsync(usuarioId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("O perfil do usuário já está cadastrado.");
        }

        PerfilUsuario perfil = PerfilUsuario.Criar(usuarioId, request.Nome);
        await repositorioPerfil.CadastrarAsync(perfil, cancellationToken);
        return Converter(perfil);
    }

    private Guid ObterUsuarioId() =>
        provedorDeUsuario.Id
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    public static PerfilUsuarioDto Converter(PerfilUsuario perfil) =>
        new(perfil.Id, perfil.UsuarioId, perfil.Nome, perfil.DataCriacaoEmUtc, perfil.DataAtualizacaoEmUtc);
}
