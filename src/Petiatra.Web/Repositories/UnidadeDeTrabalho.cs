using Petiatra.Web.Data;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Repositories;

public class UnidadeDeTrabalho(AppDbContext contexto) : IUnidadeDeTrabalho
{
    public async Task<T> ExecutarAsync<T>(Func<Task<T>> operacao, Func<T, bool> confirmar)
    {
        await using var transacao = await contexto.Database.BeginTransactionAsync();
        var resultado = await operacao();

        if (confirmar(resultado))
        {
            await transacao.CommitAsync();
        }
        else
        {
            await transacao.RollbackAsync();
            contexto.ChangeTracker.Clear();
        }

        return resultado;
    }
}
