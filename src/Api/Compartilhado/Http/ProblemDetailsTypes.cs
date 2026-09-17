namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class ProblemDetailsTypes
{
    public const string Validacao = "https://httpstatuses.com/400";
    public const string NaoAutenticado = "https://httpstatuses.com/401";
    public const string NaoAutorizado = "https://httpstatuses.com/403";
    public const string NaoEncontrado = "https://httpstatuses.com/404";
    public const string Conflito = "https://httpstatuses.com/409";
    public const string ErroInterno = "https://httpstatuses.com/500";
}