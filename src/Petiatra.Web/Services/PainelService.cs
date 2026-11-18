using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services;

/// <summary>RF12: dados do painel inicial.</summary>
public class PainelService(
    ITutorRepositorio tutores,
    IPetRepositorio pets,
    IConsultaRepositorio consultas,
    IVacinaRepositorio vacinas,
    TimeProvider relogio) : IPainelService
{
    private const int LimiteDeItens = 8;

    public async Task<PainelViewModel> ObterResumoAsync()
    {
        var hoje = relogio.Hoje();

        return new PainelViewModel
        {
            Hoje = hoje,
            TotalTutores = await tutores.ContarAsync(),
            TotalPets = await pets.ContarAsync(),
            TotalVacinasAtrasadas = await vacinas.ContarAtrasadasAsync(hoje),
            ConsultasDoDia = await consultas.ListarDoDiaAsync(hoje),
            VacinasAtrasadas = await vacinas.ListarAtrasadasAsync(hoje, LimiteDeItens),
            ProximosReforcos = await vacinas.ListarReforcosEntreAsync(hoje, hoje.AddDays(30), LimiteDeItens)
        };
    }
}
