using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Services.Interfaces;

namespace Petiatra.Web.Controllers;

/// <summary>RF12: painel inicial.</summary>
public class PainelController(IPainelService painel) : AppController
{
    public async Task<IActionResult> Index() => View(await painel.ObterResumoAsync());
}
