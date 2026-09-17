namespace GeradorCertificadosOnline.Aplicacao.Modulos.Usuarios.DTOs;

public sealed record UsuarioDto(Guid Id, string Email);

public sealed record TokenDto(string AccessToken, DateTime ExpiresAt);