using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

[AllowAnonymous]
[Route("Erro")]
public class ErroController : Controller
{
    [Route("{codigo:int?}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(int? codigo) =>
        View("~/Views/Shared/Error.cshtml", new ErrorViewModel
        {
            Codigo = codigo,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
}
