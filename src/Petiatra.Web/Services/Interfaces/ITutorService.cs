using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Services.Interfaces;

public interface ITutorService
{
    Task<Pagina<Tutor>> ListarAsync(FiltroTutores filtro);
    Task<Tutor?> ObterAsync(int id);
    Task<Tutor?> ObterComPetsAsync(int id);
    Task<Resultado<Tutor>> SalvarAsync(Tutor tutor);
    Task<Resultado> ExcluirAsync(int id);
}
