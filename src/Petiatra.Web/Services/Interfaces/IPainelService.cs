using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services.Interfaces;

public interface IPainelService
{
    Task<PainelViewModel> ObterResumoAsync();
}
