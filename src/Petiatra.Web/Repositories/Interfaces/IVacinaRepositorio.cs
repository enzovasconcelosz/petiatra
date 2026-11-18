using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Repositories.Interfaces;

public interface IVacinaRepositorio : IRepositorio<Vacina>
{
    Task<Pagina<ItemVacina>> BuscarPaginadoAsync(FiltroVacinas filtro, DateOnly hoje);
    Task<IReadOnlyList<Vacina>> ListarAtrasadasAsync(DateOnly hoje, int limite = int.MaxValue);
    Task<int> ContarAtrasadasAsync(DateOnly hoje);
    Task<IReadOnlyList<Vacina>> ListarReforcosEntreAsync(DateOnly inicio, DateOnly fim, int limite = int.MaxValue);
    Task<Vacina?> ObterCompletaAsync(int id);
    Task<IReadOnlyList<string>> ListarNomesUsadosAsync();
}
