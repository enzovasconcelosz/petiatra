namespace Petiatra.Web.Repositories.Interfaces;

/// <summary>Contrato comum de leitura e gravação de uma entidade.</summary>
public interface IRepositorio<T> where T : class
{
    Task<T?> ObterPorIdAsync(int id);
    Task AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(T entidade);
    Task<int> ContarAsync();
}
