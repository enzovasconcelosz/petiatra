using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Repositories.Interfaces;

public interface ITutorRepositorio : IRepositorio<Tutor>
{
    Task<Pagina<Tutor>> BuscarPaginadoAsync(FiltroTutores filtro);
    Task<IReadOnlyList<Tutor>> BuscarRapidoAsync(string termo, int limite);
    Task<Tutor?> ObterComPetsAsync(int id);
    Task<Tutor?> ObterPorCpfAsync(string cpf);
    Task<bool> ExisteCpfAsync(string cpf, int idIgnorado = 0);
    Task<bool> PossuiPetsAsync(int id);
}
