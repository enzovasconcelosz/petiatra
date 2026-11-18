using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services.Interfaces;

public interface IPetService
{
    Task<Pagina<Pet>> ListarAsync(FiltroPets filtro);
    Task<IReadOnlyList<Pet>> ListarDoTutorAsync(int tutorId);
    Task<Pet?> ObterAsync(int id);
    Task<FichaPetViewModel?> ObterFichaAsync(int id);
    Task<Resultado<Pet>> SalvarAsync(Pet pet);
    Task<Resultado> ExcluirAsync(int id);
}
