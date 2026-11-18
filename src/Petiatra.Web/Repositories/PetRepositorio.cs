using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class PetRepositorio(AppDbContext contexto) : Repositorio<Pet>(contexto), IPetRepositorio
{
    public Task<Pagina<Pet>> BuscarPaginadoAsync(FiltroPets filtro)
    {
        var consulta = Filtrar(Tabela.AsNoTracking().Include(p => p.Tutor), filtro.Busca);
        if (filtro.Especie is { } especie)
            consulta = consulta.Where(p => p.Especie == especie);

        return PaginarAsync(consulta.OrderBy(p => p.Nome), filtro.Pagina);
    }

    public async Task<IReadOnlyList<Pet>> BuscarRapidoAsync(string termo, int limite) =>
        await Filtrar(Tabela.AsNoTracking().Include(p => p.Tutor), termo)
            .OrderBy(p => p.Nome).Take(limite).ToListAsync();

    public async Task<IReadOnlyList<Pet>> ListarDoTutorAsync(int tutorId) =>
        await Tabela.AsNoTracking().Where(p => p.TutorId == tutorId).OrderBy(p => p.Nome).ToListAsync();

    public Task<Pet?> ObterComTutorAsync(int id) =>
        Tabela.Include(p => p.Tutor).FirstOrDefaultAsync(p => p.Id == id);

    public Task<Pet?> ObterComHistoricoAsync(int id) =>
        Tabela.AsNoTracking()
              .Include(p => p.Tutor)
              .Include(p => p.Consultas.OrderByDescending(c => c.DataHora)).ThenInclude(c => c.Veterinario)
              .Include(p => p.Vacinas.OrderByDescending(v => v.DataAplicacao)).ThenInclude(v => v.Veterinario)
              .AsSplitQuery()
              .FirstOrDefaultAsync(p => p.Id == id);

    /// <summary>Busca pelo nome do pet, pelo nome do tutor ou pelo CPF do tutor.</summary>
    private static IQueryable<Pet> Filtrar(IQueryable<Pet> consulta, string? busca)
    {
        if (string.IsNullOrWhiteSpace(busca))
            return consulta;

        var texto = Contendo(busca);
        var digitos = Documento.SomenteDigitos(busca);
        if (digitos.Length == 0)
            return consulta.Where(p => EF.Functions.Like(p.Nome, texto) || EF.Functions.Like(p.Tutor!.Nome, texto));

        var cpfParecido = $"%{digitos}%";
        return consulta.Where(p => EF.Functions.Like(p.Nome, texto)
                                   || EF.Functions.Like(p.Tutor!.Nome, texto)
                                   || EF.Functions.Like(p.Tutor!.Cpf, cpfParecido));
    }
}
