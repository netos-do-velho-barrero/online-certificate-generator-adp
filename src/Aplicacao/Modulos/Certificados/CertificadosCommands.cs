using FluentValidation;
using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
public sealed record SolicitarCertificadosCommand(Guid CursoId, IReadOnlyList<AlunoRequest> Alunos, Guid UsuarioId) : IRequest<ProcessamentoResponse>;
public sealed class SolicitarCertificadosValidator : AbstractValidator<SolicitarCertificadosCommand>
{
    public SolicitarCertificadosValidator() { RuleFor(x => x.Alunos).NotEmpty(); RuleForEach(x => x.Alunos).ChildRules(a => a.RuleFor(x => x.Nome).NotEmpty().MaximumLength(200)); }
}
public sealed class SolicitarCertificadosHandler(IRepositorioCurso cursos, IRepositorioProcessamento repositorios, IFilaCertificados fila) : IRequestHandler<SolicitarCertificadosCommand, ProcessamentoResponse>
{
    public async Task<ProcessamentoResponse> Handle(SolicitarCertificadosCommand r, CancellationToken ct)
    {
        if (await cursos.SelecionarPorIdAsync(r.CursoId, ct) is null)
        {
            throw new KeyNotFoundException("Curso não encontrado.");
        }
        if (await repositorios.PossuiAtivoAsync(r.CursoId, r.UsuarioId, ct))
        {
            throw new ConflitoDeRegraDeNegocioException(
                "Já existe processamento em andamento para este curso."
            );
        }
        var p = ProcessamentoCertificados.Criar(r.CursoId, r.UsuarioId);
        foreach (var a in r.Alunos) p.Certificados.Add(Certificado.Criar(p.Id, p.CursoId, p.UsuarioId, a.Nome));
        await repositorios.CadastrarAsync(p, ct);
        await fila.EnfileirarAsync(p.Id, ct);
        return new(p.Id, p.Status.ToString());
    }
}
