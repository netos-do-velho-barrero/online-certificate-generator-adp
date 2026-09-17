using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios;
using GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Usuarios;

[ApiController]
[Route("auth")]
public sealed class UsuariosController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType(typeof(CadastroUsuarioResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Cadastrar(
        CadastroUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        UsuarioDto usuario = await sender.Send(
            new CadastrarUsuarioCommand(request.Email, request.Senha),
            cancellationToken
        );
        return CreatedAtAction(
            nameof(Cadastrar),
            new { id = usuario.Id },
            new CadastroUsuarioResponse(usuario.Id, usuario.Email)
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUsuarioResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        LoginUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        TokenDto? token = await sender.Send(
            new AutenticarUsuarioCommand(request.Email, request.Senha),
            cancellationToken
        );
        return token is null
            ? Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Não autenticado",
                Detail = "E-mail ou senha inválidos."
            })
            : Ok(new LoginUsuarioResponse(token.AccessToken, token.ExpiresAt));
    }

    [Authorize]
    [HttpGet("me/perfil")]
    public async Task<ActionResult<PerfilUsuarioDto>> ObterMeuPerfil(
        CancellationToken cancellationToken
    )
    {
        return Ok(await sender.Send(new ObterMeuPerfilQuery(), cancellationToken));
    }

    [Authorize]
    [HttpPost("me/perfil")]
    public async Task<ActionResult<PerfilUsuarioDto>> CadastrarMeuPerfil(
        PerfilUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        PerfilUsuarioDto perfil = await sender.Send(
            new CadastrarPerfilUsuarioCommand(request.Nome),
            cancellationToken
        );
        return CreatedAtAction(nameof(ObterMeuPerfil), perfil);
    }

    [Authorize]
    [HttpPut("me/perfil")]
    public async Task<ActionResult<PerfilUsuarioDto>> EditarMeuPerfil(
        PerfilUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(await sender.Send(
            new EditarPerfilUsuarioCommand(request.Nome),
            cancellationToken
        ));
    }
}