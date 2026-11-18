using Petiatra.Web.Models;

namespace Petiatra.Web.Services.Interfaces;

public interface IAuthService
{
    Task<Resultado<Veterinario>> AutenticarAsync(string email, string senha);
    Task<Resultado<Veterinario>> CadastrarAsync(Veterinario veterinario, string senha);
    Task<IReadOnlyList<Veterinario>> ListarVeterinariosAsync();
}
