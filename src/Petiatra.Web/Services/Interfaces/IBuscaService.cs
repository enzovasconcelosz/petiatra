using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services.Interfaces;

public interface IBuscaService
{
    Task<BuscaViewModel> BuscarAsync(string? termo, int limite);
}
