using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Auth;

public sealed class GerenciadorDeIdentidade(
    UserManager<IdentityUser<Guid>> gerenciadorDeUsuarios,
    RoleManager<IdentityRole<Guid>> gerenciadorDePerfis
) : IGerenciadorDeIdentidade
{
    public async Task<UsuarioDto> CadastrarAsync(
        Guid usuarioId,
        string email,
        string senha,
        TipoUsuario tipo
    )
    {
        IdentityUser<Guid> usuario = new()
        {
            Id = usuarioId,
            Email = email,
            UserName = email
        };

        IdentityResult resultado = await gerenciadorDeUsuarios.CreateAsync(usuario, senha);
        if (!resultado.Succeeded)
        {
            IdentityError? conflito = resultado.Errors.FirstOrDefault(erro =>
                erro.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)
            );

            if (conflito is not null)
            {
                throw new ConflitoDeIdentidadeException("O e-mail informado já está cadastrado.");
            }

            IdentityError? erroSenha = resultado.Errors.FirstOrDefault(erro =>
                erro.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)
            );

            throw new ValidacaoDeIdentidadeException(
                erroSenha is null ? "Email" : "Senha",
                string.Join(" ", resultado.Errors.Select(erro => erro.Description))
            );
        }

        string nomeDoPerfil = tipo.ToString();
        if (!await gerenciadorDePerfis.RoleExistsAsync(nomeDoPerfil))
        {
            await gerenciadorDeUsuarios.DeleteAsync(usuario);
            throw new InvalidOperationException($"O perfil '{nomeDoPerfil}' não está configurado.");
        }

        IdentityResult resultadoPerfil = await gerenciadorDeUsuarios.AddToRoleAsync(
            usuario,
            nomeDoPerfil
        );
        if (!resultadoPerfil.Succeeded)
        {
            await gerenciadorDeUsuarios.DeleteAsync(usuario);
            throw new InvalidOperationException(
                "Não foi possível associar o perfil ao usuário."
            );
        }

        return new UsuarioDto(usuario.Id, usuario.Email!);
    }

    public async Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha,
        TipoUsuario tipo
    )
    {
        IdentityUser<Guid>? usuario = await gerenciadorDeUsuarios.FindByEmailAsync(email);
        if (usuario is null || !await gerenciadorDeUsuarios.CheckPasswordAsync(usuario, senha))
        {
            return null;
        }

        if (!await gerenciadorDeUsuarios.IsInRoleAsync(usuario, tipo.ToString()))
        {
            return null;
        }

        return new UsuarioDto(usuario.Id, usuario.Email!);
    }

    public async Task ExcluirAsync(Guid usuarioId)
    {
        IdentityUser<Guid>? usuario = await gerenciadorDeUsuarios.FindByIdAsync(
            usuarioId.ToString()
        );
        if (usuario is null)
        {
            return;
        }

        IdentityResult resultado = await gerenciadorDeUsuarios.DeleteAsync(usuario);
        if (!resultado.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(" ", resultado.Errors.Select(erro => erro.Description))
            );
        }
    }
}