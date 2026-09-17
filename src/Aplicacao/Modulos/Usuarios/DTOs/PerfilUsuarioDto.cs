namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;

public sealed record PerfilUsuarioDto(
    Guid Id,
    Guid UsuarioId,
    string Nome,
    DateTime DataCriacaoEmUtc,
    DateTime DataAtualizacaoEmUtc
);
