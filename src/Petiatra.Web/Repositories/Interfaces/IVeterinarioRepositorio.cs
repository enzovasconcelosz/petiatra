using Petiatra.Web.Models;

namespace Petiatra.Web.Repositories.Interfaces;

public interface IVeterinarioRepositorio : IRepositorio<Veterinario>
{
    Task<Veterinario?> ObterPorEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email, int idIgnorado = 0);
    Task<bool> ExisteCrmvAsync(string crmv, int idIgnorado = 0);
    Task<IReadOnlyList<Veterinario>> ListarTodosAsync();
}
