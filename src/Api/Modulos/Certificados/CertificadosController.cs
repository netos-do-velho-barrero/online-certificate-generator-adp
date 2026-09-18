using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Certificados;

[ApiController]
[Authorize]
[Route("cursos/{cursoId:guid}")]
public sealed class CertificadosController(
    ISender sender,
    IProvedorDeUsuario provedorDeUsuario
) : ControllerBase
{
    [HttpPost("certificados")]
    [ProducesResponseType(typeof(ProcessamentoResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Solicitar(
        Guid cursoId,
        SolicitarCertificadosHttpRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid usuarioId = ObterUsuarioId();
        ProcessamentoResponse response = await sender.Send(
            new SolicitarCertificadosCommand(cursoId, request.Alunos, usuarioId),
            cancellationToken
        );

        return AcceptedAtAction(
            nameof(ObterStatus),
            new { cursoId },
            response
        );
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusResponse>> ObterStatus(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        return Ok(await sender.Send(
            new ObterStatusCertificadosQuery(cursoId, ObterUsuarioId()),
            cancellationToken
        ));
    }

    [HttpGet("certificados")]
    [ProducesResponseType(typeof(IReadOnlyList<CertificadoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CertificadoResponse>>> Listar(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        return Ok(await sender.Send(
            new ListarCertificadosQuery(cursoId, ObterUsuarioId()),
            cancellationToken
        ));
    }

    [HttpGet("certificados/download")]
    [Produces("application/zip")]
    public async Task<IActionResult> Download(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        ArquivoDownloadResponse arquivo = await sender.Send(
            new DownloadCertificadosQuery(cursoId, ObterUsuarioId()),
            cancellationToken
        );
        return File(arquivo.Conteudo, "application/zip", arquivo.NomeArquivo);
    }

    private Guid ObterUsuarioId() =>
        provedorDeUsuario.Id
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
}