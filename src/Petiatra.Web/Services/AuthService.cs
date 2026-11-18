using Microsoft.AspNetCore.Identity;
using Petiatra.Web.Models;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;

namespace Petiatra.Web.Services;

public class AuthService(
    IVeterinarioRepositorio veterinarios,
    IPasswordHasher<Veterinario> hasher,
    TimeProvider relogio) : IAuthService
{
    private const string MensagemCredenciaisInvalidas = "E-mail ou senha inválidos.";

    public async Task<Resultado<Veterinario>> AutenticarAsync(string email, string senha)
    {
        var veterinario = await veterinarios.ObterPorEmailAsync(NormalizarEmail(email));
        if (veterinario is null)
            return Resultado<Veterinario>.Falha(MensagemCredenciaisInvalidas);

        var verificacao = hasher.VerifyHashedPassword(veterinario, veterinario.SenhaHash, senha);
        if (verificacao == PasswordVerificationResult.Failed)
            return Resultado<Veterinario>.Falha(MensagemCredenciaisInvalidas);

        if (verificacao == PasswordVerificationResult.SuccessRehashNeeded)
        {
            veterinario.DefinirSenha(hasher.HashPassword(veterinario, senha));
            await veterinarios.AtualizarAsync(veterinario);
        }

        return Resultado<Veterinario>.Ok(veterinario);
    }

    public async Task<Resultado<Veterinario>> CadastrarAsync(Veterinario veterinario, string senha)
    {
        veterinario.Nome = veterinario.Nome.Trim();
        veterinario.Email = NormalizarEmail(veterinario.Email);
        veterinario.Crmv = veterinario.Crmv.Trim().ToUpperInvariant();

        // RN03: e-mail e CRMV únicos
        if (await veterinarios.ExisteEmailAsync(veterinario.Email))
            return Resultado<Veterinario>.Falha("Já existe um veterinário com este e-mail.", nameof(Veterinario.Email));

        if (await veterinarios.ExisteCrmvAsync(veterinario.Crmv))
            return Resultado<Veterinario>.Falha("Já existe um veterinário com este CRMV.", nameof(Veterinario.Crmv));

        veterinario.DefinirSenha(hasher.HashPassword(veterinario, senha));
        veterinario.CriadoEm = relogio.Agora();
        await veterinarios.AdicionarAsync(veterinario);

        return Resultado<Veterinario>.Ok(veterinario);
    }

    public Task<IReadOnlyList<Veterinario>> ListarVeterinariosAsync() => veterinarios.ListarTodosAsync();

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
