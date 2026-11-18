namespace Petiatra.Web.Repositories.Interfaces;

/// <summary>Executa várias gravações como uma só: ou todas são confirmadas, ou nenhuma.</summary>
public interface IUnidadeDeTrabalho
{
    Task<T> ExecutarAsync<T>(Func<Task<T>> operacao, Func<T, bool> confirmar);
}
