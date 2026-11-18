using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

/// <summary>Implementação base de <see cref="IRepositorio{T}"/> sobre o Entity Framework Core.</summary>
public abstract class Repositorio<T>(AppDbContext contexto) : IRepositorio<T> where T : class
{
    protected AppDbContext Contexto { get; } = contexto;
    protected DbSet<T> Tabela => Contexto.Set<T>();

    public virtual Task<T?> ObterPorIdAsync(int id) => Tabela.FindAsync(id).AsTask();

    public async Task AdicionarAsync(T entidade)
    {
        Tabela.Add(entidade);
        await Contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(T entidade)
    {
        Tabela.Update(entidade);
        await Contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(T entidade)
    {
        Tabela.Remove(entidade);
        await Contexto.SaveChangesAsync();
    }

    public Task<int> ContarAsync() => Tabela.CountAsync();

    protected static async Task<Models.Comum.Pagina<TItem>> PaginarAsync<TItem>(IQueryable<TItem> consulta, int numero)
    {
        var pagina = Models.Comum.Pagina<TItem>.Normalizar(numero);
        var tamanho = Models.Comum.Pagina<TItem>.TamanhoPadrao;
        var total = await consulta.CountAsync();
        var itens = await consulta.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync();
        return new Models.Comum.Pagina<TItem>(itens, pagina, tamanho, total);
    }

    protected static string Contendo(string termo) => $"%{termo.Trim()}%";
}
