using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Repositories.Interfaces;

public interface IPetRepositorio : IRepositorio<Pet>
{
    Task<Pagina<Pet>> BuscarPaginadoAsync(FiltroPets filtro);
    Task<IReadOnlyList<Pet>> BuscarRapidoAsync(string termo, int limite);
    Task<IReadOnlyList<Pet>> ListarDoTutorAsync(int tutorId);
    Task<Pet?> ObterComTutorAsync(int id);
    Task<Pet?> ObterComHistoricoAsync(int id);
}
