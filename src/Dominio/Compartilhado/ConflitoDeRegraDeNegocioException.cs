namespace GeradorCertificadosOnline.Dominio.Compartilhado;

public sealed class ConflitoDeRegraDeNegocioException(string mensagem) : Exception(mensagem);
