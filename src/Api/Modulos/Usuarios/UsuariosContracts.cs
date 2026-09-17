namespace GeradorCertificadosOnline.Api.Modulos.Usuarios;

public sealed record CadastroUsuarioRequest(string Email, string Senha);
public sealed record LoginUsuarioRequest(string Email, string Senha);
public sealed record CadastroUsuarioResponse(Guid Id, string Email);
public sealed record LoginUsuarioResponse(string AccessToken, DateTime ExpiresAt);
public sealed record PerfilUsuarioRequest(string Nome);