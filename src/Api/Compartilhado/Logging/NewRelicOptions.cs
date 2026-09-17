namespace GeradorCertificadosOnline.Api.Compartilhado.Logging;

public sealed class NewRelicOptions
{
    public bool Enabled { get; set; }
    public string? EndpointUrl { get; set; }
    public string? ApplicationName { get; set; }
}