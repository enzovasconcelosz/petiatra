using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Repositories.Interfaces;

public interface IConsultaRepositorio : IRepositorio<Consulta>
{
    Task<Pagina<Consulta>> BuscarPaginadoAsync(FiltroConsultas filtro);
    Task<IReadOnlyList<Consulta>> ListarDoVeterinarioNoDiaAsync(int veterinarioId, DateOnly dia);
    Task<IReadOnlyList<Consulta>> ListarDoDiaAsync(DateOnly dia);
    Task<Consulta?> ObterCompletaAsync(int id);
}
