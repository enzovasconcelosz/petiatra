using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Services.Interfaces;

public interface IVacinaService
{
    Task<Pagina<ItemVacina>> ListarAsync(FiltroVacinas filtro);
    Task<IReadOnlyList<Vacina>> ListarAtrasadasAsync();
    Task<IReadOnlyList<string>> ListarNomesSugeridosAsync();
    Task<Vacina?> ObterAsync(int id);
    Task<Resultado<Vacina>> RegistrarAsync(Vacina vacina);
    Task<Resultado<Vacina>> AtualizarAsync(Vacina vacina);
    Task<Resultado> ExcluirAsync(int id);
}
