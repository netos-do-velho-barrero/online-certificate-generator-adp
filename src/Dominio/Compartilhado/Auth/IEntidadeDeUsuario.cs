namespace GeradorCertificadosOnline.Dominio.Compartilhado.Auth;

// Garante que determinadas entidades tenham um UsuarioId para identificar a quem pertencem.
public interface IEntidadeDeUsuario
{
    Guid UsuarioId { get; set; }
}