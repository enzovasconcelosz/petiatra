using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services;

/// <summary>RF14: busca rápida global por tutor, pet ou CPF.</summary>
public class BuscaService(ITutorRepositorio tutores, IPetRepositorio pets) : IBuscaService
{
    public async Task<BuscaViewModel> BuscarAsync(string? termo, int limite)
    {
        if (string.IsNullOrWhiteSpace(termo) || termo.Trim().Length < 2)
            return new BuscaViewModel(termo ?? string.Empty, [], []);

        var texto = termo.Trim();
        return new BuscaViewModel(
            texto,
            await tutores.BuscarRapidoAsync(texto, limite),
            await pets.BuscarRapidoAsync(texto, limite));
    }
}
