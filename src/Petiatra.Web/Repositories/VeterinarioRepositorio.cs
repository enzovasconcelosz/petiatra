using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class VeterinarioRepositorio(AppDbContext contexto) : Repositorio<Veterinario>(contexto), IVeterinarioRepositorio
{
    public Task<Veterinario?> ObterPorEmailAsync(string email) =>
        Tabela.FirstOrDefaultAsync(v => v.Email == email);

    public Task<bool> ExisteEmailAsync(string email, int idIgnorado = 0) =>
        Tabela.AnyAsync(v => v.Email == email && v.Id != idIgnorado);

    public Task<bool> ExisteCrmvAsync(string crmv, int idIgnorado = 0) =>
        Tabela.AnyAsync(v => v.Crmv == crmv && v.Id != idIgnorado);

    public async Task<IReadOnlyList<Veterinario>> ListarTodosAsync() =>
        await Tabela.AsNoTracking().OrderBy(v => v.Nome).ToListAsync();
}
