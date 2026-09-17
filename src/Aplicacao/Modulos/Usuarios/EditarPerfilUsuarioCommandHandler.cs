using FluentValidation;
using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;

public sealed record EditarPerfilUsuarioCommand(string Nome) : IRequest<PerfilUsuarioDto>;

public sealed class EditarPerfilUsuarioCommandValidator
    : AbstractValidator<EditarPerfilUsuarioCommand>
{
    public EditarPerfilUsuarioCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
    }
}

public sealed class EditarPerfilUsuarioCommandHandler(
    IProvedorDeUsuario provedorDeUsuario,
    IRepositorioPerfilUsuario repositorioPerfil
) : IRequestHandler<EditarPerfilUsuarioCommand, PerfilUsuarioDto>
{
    public async Task<PerfilUsuarioDto> Handle(
        EditarPerfilUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid usuarioId = provedorDeUsuario.Id
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
        PerfilUsuario perfil = await repositorioPerfil.SelecionarPorUsuarioIdAsync(
            usuarioId,
            cancellationToken
        ) ?? throw new KeyNotFoundException("Perfil do usuário não encontrado.");

        perfil.AlterarNome(request.Nome);
        await repositorioPerfil.EditarAsync(perfil.Id, perfil, cancellationToken);
        return CadastrarPerfilUsuarioCommandHandler.Converter(perfil);
    }
}