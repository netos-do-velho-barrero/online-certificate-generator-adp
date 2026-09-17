using System.Security.Claims;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;

namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

public sealed class UserProvider(IHttpContextAccessor accessor) : IProvedorDeUsuario
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid? Id
    {
        get
        {
            string? value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Principal?.FindFirstValue("sub");
            return Guid.TryParse(value, out Guid id) ? id : null;
        }
    }

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email);
    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true;

    public bool PossuiTipo(TipoUsuario tipoUsuario) =>
        Principal?.IsInRole(tipoUsuario.ToString()) == true;
}