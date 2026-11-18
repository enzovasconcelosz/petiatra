using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class VacinaRepositorio(AppDbContext contexto) : Repositorio<Vacina>(contexto), IVacinaRepositorio
{
    public Task<Pagina<ItemVacina>> BuscarPaginadoAsync(FiltroVacinas filtro, DateOnly hoje)
    {
        var consulta = filtro.Situacao switch
        {
            FiltroSituacaoVacina.Atrasadas => Atrasadas(hoje),
            FiltroSituacaoVacina.ProximosReforcos => ReforcosEntre(hoje, hoje.AddDays(30)),
            _ => Tabela.AsQueryable()
        };

        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var texto = Contendo(filtro.Busca);
            consulta = consulta.Where(v => EF.Functions.Like(v.Nome, texto)
                                           || EF.Functions.Like(v.Pet!.Nome, texto)
                                           || EF.Functions.Like(v.Pet!.Tutor!.Nome, texto));
        }

        var ordenada = filtro.Situacao == FiltroSituacaoVacina.Todas
            ? consulta.OrderByDescending(v => v.DataAplicacao)
            : consulta.OrderBy(v => v.DataReforco);

        var itens = ordenada
            .Include(v => v.Pet).ThenInclude(p => p!.Tutor)
            .Include(v => v.Veterinario)
            .AsNoTracking()
            .Select(v => new ItemVacina(v, Tabela.Any(o => o.PetId == v.PetId
                                                         && o.Nome == v.Nome
                                                         && o.DataAplicacao > v.DataAplicacao)));

        return PaginarAsync(itens, filtro.Pagina);
    }

    public async Task<IReadOnlyList<Vacina>> ListarAtrasadasAsync(DateOnly hoje, int limite = int.MaxValue) =>
        await ComPetETutor(Atrasadas(hoje)).OrderBy(v => v.DataReforco).Take(limite).ToListAsync();

    public Task<int> ContarAtrasadasAsync(DateOnly hoje) => Atrasadas(hoje).CountAsync();

    public async Task<IReadOnlyList<Vacina>> ListarReforcosEntreAsync(DateOnly inicio, DateOnly fim, int limite = int.MaxValue) =>
        await ComPetETutor(ReforcosEntre(inicio, fim)).OrderBy(v => v.DataReforco).Take(limite).ToListAsync();

    public Task<Vacina?> ObterCompletaAsync(int id) =>
        Tabela.Include(v => v.Pet).ThenInclude(p => p!.Tutor).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<IReadOnlyList<string>> ListarNomesUsadosAsync() =>
        await Tabela.Select(v => v.Nome).Distinct().OrderBy(n => n).ToListAsync();

    /// <summary>RN07: reforço vencido e nenhuma dose posterior da mesma vacina no mesmo pet.</summary>
    private IQueryable<Vacina> Atrasadas(DateOnly hoje) =>
        SemDosePosterior(Tabela.Where(v => v.DataReforco != null && v.DataReforco < hoje));

    private IQueryable<Vacina> ReforcosEntre(DateOnly inicio, DateOnly fim) =>
        SemDosePosterior(Tabela.Where(v => v.DataReforco != null && v.DataReforco >= inicio && v.DataReforco <= fim));

    private IQueryable<Vacina> SemDosePosterior(IQueryable<Vacina> consulta) =>
        consulta.Where(v => !Tabela.Any(o => o.PetId == v.PetId
                                             && o.Nome == v.Nome
                                             && o.DataAplicacao > v.DataAplicacao));

    private static IQueryable<Vacina> ComPetETutor(IQueryable<Vacina> consulta) =>
        consulta.AsNoTracking().Include(v => v.Pet).ThenInclude(p => p!.Tutor);
}
