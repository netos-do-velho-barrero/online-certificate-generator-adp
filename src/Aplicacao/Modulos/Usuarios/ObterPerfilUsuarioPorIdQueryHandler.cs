using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Usuarios;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;

public sealed record ObterMeuPerfilQuery : IRequest<PerfilUsuarioDto>;

public sealed class ObterMeuPerfilQueryHandler(
    IProvedorDeUsuario provedorDeUsuario,
    IRepositorioPerfilUsuario repositorioPerfil
) : IRequestHandler<ObterMeuPerfilQuery, PerfilUsuarioDto>
{
    public async Task<PerfilUsuarioDto> Handle(
        ObterMeuPerfilQuery request,
        CancellationToken cancellationToken
    )
    {
        Guid usuarioId = provedorDeUsuario.Id
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
        PerfilUsuario perfil = await repositorioPerfil.SelecionarPorUsuarioIdAsync(
            usuarioId,
            cancellationToken
        ) ?? throw new KeyNotFoundException("Perfil do usuário não encontrado.");

        return CadastrarPerfilUsuarioCommandHandler.Converter(perfil);
    }
}
