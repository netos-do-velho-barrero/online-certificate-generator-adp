using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Cursos;

public sealed class RepositorioCursoEmOrm(
    GeradorCertificadosOnlineDbContext contexto
) : RepositorioBaseEmOrm<Curso>(contexto), IRepositorioCurso
{
}
