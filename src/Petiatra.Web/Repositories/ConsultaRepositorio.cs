using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class ConsultaRepositorio(AppDbContext contexto) : Repositorio<Consulta>(contexto), IConsultaRepositorio
{
    public Task<Pagina<Consulta>> BuscarPaginadoAsync(FiltroConsultas filtro)
    {
        var consulta = ComRelacionamentos(Tabela.AsNoTracking());

        if (filtro.Data is { } data)
        {
            var (inicio, fim) = Intervalo(data);
            consulta = consulta.Where(c => c.DataHora >= inicio && c.DataHora < fim);
        }

        if (filtro.VeterinarioId is { } veterinarioId)
            consulta = consulta.Where(c => c.VeterinarioId == veterinarioId);

        if (filtro.Status is { } status)
            consulta = consulta.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var texto = Contendo(filtro.Busca);
            consulta = consulta.Where(c => EF.Functions.Like(c.Pet!.Nome, texto)
                                           || EF.Functions.Like(c.Pet!.Tutor!.Nome, texto)
                                           || EF.Functions.Like(c.Motivo, texto));
        }

        // com um dia escolhido, a agenda fica em ordem de horário; sem ele, as mais recentes primeiro
        var ordenada = filtro.Data is null
            ? consulta.OrderByDescending(c => c.DataHora)
            : consulta.OrderBy(c => c.DataHora);

        return PaginarAsync(ordenada, filtro.Pagina);
    }

    public async Task<IReadOnlyList<Consulta>> ListarDoVeterinarioNoDiaAsync(int veterinarioId, DateOnly dia)
    {
        var (inicio, fim) = Intervalo(dia);
        return await Tabela.AsNoTracking()
            .Where(c => c.VeterinarioId == veterinarioId
                        && c.Status != StatusConsulta.Cancelada
                        && c.DataHora >= inicio && c.DataHora < fim)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Consulta>> ListarDoDiaAsync(DateOnly dia)
    {
        var (inicio, fim) = Intervalo(dia);
        return await ComRelacionamentos(Tabela.AsNoTracking())
            .Where(c => c.DataHora >= inicio && c.DataHora < fim)
            .OrderBy(c => c.DataHora)
            .ToListAsync();
    }

    public Task<Consulta?> ObterCompletaAsync(int id) =>
        ComRelacionamentos(Tabela).FirstOrDefaultAsync(c => c.Id == id);

    private static IQueryable<Consulta> ComRelacionamentos(IQueryable<Consulta> consulta) =>
        consulta.Include(c => c.Pet).ThenInclude(p => p!.Tutor).Include(c => c.Veterinario);

    private static (DateTime Inicio, DateTime Fim) Intervalo(DateOnly dia)
    {
        var inicio = dia.ToDateTime(TimeOnly.MinValue);
        return (inicio, inicio.AddDays(1));
    }
}
