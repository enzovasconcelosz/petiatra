using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class TutorRepositorio(AppDbContext contexto) : Repositorio<Tutor>(contexto), ITutorRepositorio
{
    public Task<Pagina<Tutor>> BuscarPaginadoAsync(FiltroTutores filtro)
    {
        var consulta = Filtrar(Tabela.AsNoTracking().Include(t => t.Pets), filtro.Busca)
            .OrderBy(t => t.Nome);
        return PaginarAsync(consulta, filtro.Pagina);
    }

    public async Task<IReadOnlyList<Tutor>> BuscarRapidoAsync(string termo, int limite) =>
        await Filtrar(Tabela.AsNoTracking(), termo).OrderBy(t => t.Nome).Take(limite).ToListAsync();

    public Task<Tutor?> ObterComPetsAsync(int id) =>
        Tabela.Include(t => t.Pets.OrderBy(p => p.Nome)).FirstOrDefaultAsync(t => t.Id == id);

    public Task<Tutor?> ObterPorCpfAsync(string cpf) =>
        Tabela.FirstOrDefaultAsync(t => t.Cpf == cpf);

    public Task<bool> ExisteCpfAsync(string cpf, int idIgnorado = 0) =>
        Tabela.AnyAsync(t => t.Cpf == cpf && t.Id != idIgnorado);

    public Task<bool> PossuiPetsAsync(int id) =>
        Contexto.Pets.AnyAsync(p => p.TutorId == id);

    /// <summary>RF04: busca por nome, CPF ou telefone.</summary>
    private static IQueryable<Tutor> Filtrar(IQueryable<Tutor> consulta, string? busca)
    {
        if (string.IsNullOrWhiteSpace(busca))
            return consulta;

        var texto = Contendo(busca);
        var digitos = Documento.SomenteDigitos(busca);
        if (digitos.Length == 0)
            return consulta.Where(t => EF.Functions.Like(t.Nome, texto));

        var digitosParecidos = $"%{digitos}%";
        return consulta.Where(t => EF.Functions.Like(t.Nome, texto)
                                   || EF.Functions.Like(t.Cpf, digitosParecidos)
                                   || EF.Functions.Like(t.Telefone, texto)
                                   || EF.Functions.Like(t.Telefone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", ""), digitosParecidos));
    }
}
